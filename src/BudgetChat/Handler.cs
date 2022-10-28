using Common.Library.Client;
using System.Net;

namespace BudgetChat
{
    internal class Handler
    {
        private Dictionary<(IPAddress, int), ClientInfo> _chatRoom;

        public Handler()
        {
            _chatRoom = new();
        }

        public async Task ProcessRequestsAsync(ClientInfo info)
        {
            _ = info.Reader ?? throw new ArgumentNullException(nameof(info.Reader));
            _ = info.Writer ?? throw new ArgumentNullException(nameof(info.Writer));

            string? username;
            string? chatMessage;

            await info.Writer.WriteLineAsync("Welcome to budgetchat! What shall I call you?").ConfigureAwait(false);

            username = await info.Reader.ReadLineAsync().ConfigureAwait(false);

            if (username == null || !IsUsernameValid(username))
            {
                await info.Writer.WriteLineAsync("Invalid username.").ConfigureAwait(false);
                info.ServerForceDisconnect = true;
                return;
            }
            info.Name = username ?? String.Empty;

            await info.Writer.WriteLineAsync($"* The room contains: {string.Join(", ", GetRoomRoster())}").ConfigureAwait(false);
            await AnnounceJoinMessageAsync(info.Name).ConfigureAwait(false);
            _chatRoom[(info.GetIP(), info.Port)] = info;

            while ((chatMessage = await info.Reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                await SendChatMessageAsync(chatMessage, info);
            }
        }

        public void RemoveUserFromChatRoom(ClientInfo info)
        {
            _chatRoom.Remove((info.GetIP(), info.Port));
        }

        public async Task AnnounceJoinMessageAsync(string username)
        {
            string announceMessage = $"* {username} has entered the room";
            await SendMessageToRoomAsync(announceMessage).ConfigureAwait(false);
        }

        public async Task AnnounceLeaveMessageAsync(string username)
        {
            string announceMessage = $"* {username} has left the room";
            await SendMessageToRoomAsync(announceMessage).ConfigureAwait(false);
        }

        private async Task SendChatMessageAsync(string chatMessage, ClientInfo info)
        {
            string message = $"[{info.Name}] {chatMessage}";
            await SendMessageToRoomAsync(message, info).ConfigureAwait(false);
        }

        private async Task SendMessageToRoomAsync(string message, ClientInfo? clientToNotSendMessage=null)
        {
            foreach (var kv in _chatRoom)
            {
                if (clientToNotSendMessage != null && kv.Key == (clientToNotSendMessage.GetIP(), clientToNotSendMessage.Port))
                {
                    continue;
                }
                ClientInfo info = kv.Value;
                if (info != clientToNotSendMessage && info.Writer != null)
                {
                    await info.Writer.WriteLineAsync(message).ConfigureAwait(false);
                }
            }
        }

        private List<string> GetRoomRoster()
        {
            return _chatRoom.Select(x => x.Value.Name ?? String.Empty).ToList();
        }

        private bool IsUsernameValid(string username)
        {
            if (username.Length > 1 && username.Length <= 16)
            {
                return true;
            }
            return false;
        }
    }
}
