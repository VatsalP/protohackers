using Common.Library;
using Common.Library.Client;
using System.Net;
using System.Net.Sockets;

namespace MobIntheMiddle
{
    internal class MobInTheMiddleTcpServer : TcpServer
    {
        private Handler _handler;

        public MobInTheMiddleTcpServer(IPAddress localaddr, int port) : base(localaddr, port)
        {
            _handler = new Handler();
        }

        public override async Task HandleClientAsync(TcpClient client, ClientInfo info)
        {
            TcpClient tcpClient = new("chat.protohackers.com", 16963);
            try
            {
                await Task.WhenAll(
                    _handler.ProcessDownstreamRequestsAsync(info, tcpClient),
                    _handler.ProccessUpstreamRequestsAsync(info, tcpClient));
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Socket Error: {e}");
            }
            finally
            {
                tcpClient.Close();
                client.Close();
                Console.WriteLine($"Client disconnected: IP {info.Ip} Port {info.Port}");
            }
        }
    }
}
