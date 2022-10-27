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

            info.Writer.WriteLine("Welcome to budgetchat! What shall I call you?");

            username = info.Reader.ReadLine();

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
                await SendMessageToRoomAsync(chatMessage);
                chatMessage = String.Empty;
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

        private async Task SendMessageToRoomAsync(string message)
        {
            foreach (ClientInfo info in _chatRoom.Values)
            {
                if (info.Writer != null)
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
