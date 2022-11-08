using Common.Library;
using Common.Library.Client;
using System.Net;
using System.Net.Sockets;

namespace MeansToAnEnd
{
    internal class MeansEndTcpServer : TcpServer
    {
        private MeansEndHandler _handler;

        public MeansEndTcpServer(IPAddress localaddr, int port) : base(localaddr, port)
        {
            _handler = new MeansEndHandler();
        }

        public override async Task HandleClientAsync(TcpClient client, ClientInfo info)
        {
            try
            {
                await _handler.ProcessRequestsAsync(info).ConfigureAwait(false);
            }
            catch (EndOfStreamException e)
            {
                Console.WriteLine($"Socket Error: {e}");
                await info.Writer.WriteAsync("Out of bounds").ConfigureAwait(false);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Socket Error: {e}");
            }
            finally
            {
                client.Close();
                Console.WriteLine($"Client disconnected: IP {info.Ip} Port {info.Port}; Request Count: {info.RequestCount}");
            }
        }
    }
}
