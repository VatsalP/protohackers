using Common.Library.Client;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MobIntheMiddle
{
    internal class Handler
    {
        private const string pattern = @"^7[0-9a-zA-z]{25,34}$";

        public Handler()
        {
        }

        public async Task ProcessDownstreamRequestsAsync(ClientInfo info, TcpClient tcpClient)
        {
            _ = info.Reader ?? throw new ArgumentNullException(nameof(info.Reader));

            using StreamWriter upstreamWriter = new(tcpClient.GetStream());

            string? chatMessage;
            while ((chatMessage = await ReadUntilNewlineAsync(info.Reader).ConfigureAwait(false)) != null)
            {
                Console.WriteLine($"Message from client: {chatMessage}");
                chatMessage = FindAndReplaceBogusAddress(chatMessage);
                await SendChatMessageAsync(chatMessage, upstreamWriter).ConfigureAwait(false);
            }
        }

        public async Task ProccessUpstreamRequestsAsync(ClientInfo info, TcpClient tcpClient)
        {
            _ = info.Writer ?? throw new ArgumentNullException(nameof(info.Writer));

            using StreamReader upstreamReader = new(tcpClient.GetStream());

            string? chatMessage;
            while ((chatMessage = await upstreamReader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                Console.WriteLine($"Message from server: {chatMessage}");
                chatMessage = FindAndReplaceBogusAddress(chatMessage);
                await SendChatMessageAsync(chatMessage, info.Writer).ConfigureAwait(false);
            }
        }

        private string FindAndReplaceBogusAddress(string chatMessage)
        {
            return string.Join(' ', chatMessage.Split(' ').Select(x => Regex.IsMatch(x, pattern) ? "7YWHMfk9JZe0LM0g1ZauHuiSxhI" : x));
        }

        private async Task SendChatMessageAsync(string chatMessage, StreamWriter writer)
        {
            await writer.WriteLineAsync(chatMessage).ConfigureAwait(false);
            await writer.FlushAsync().ConfigureAwait(false);
        }


        /// <summary>
        /// ReadLineAsync send partial data even if eof has reached and no new line char
        /// was given, so made this method for user reads
        /// </summary>
        /// <param name="reader">StreamReader</param>
        /// <returns>String sent by user or null</returns>
        private async Task<string?> ReadUntilNewlineAsync(StreamReader reader)
        {
            char[] buffer = new char[1];
            StringBuilder sb = new();
            while (await reader.ReadAsync(buffer, 0, 1) != 0) {
                if (buffer[0].Equals('\n'))
                {
                    return sb.ToString();
                }
                sb.Append(buffer[0]);
            }
            return null;
        }
    }
}
