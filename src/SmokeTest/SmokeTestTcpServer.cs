using Common.Library;
using Common.Library.Client;
using System.Net;
using System.Net.Sockets;

namespace SmokeTest;

class SmokeTestTcpServer : TcpServer
{
    public SmokeTestTcpServer(IPAddress localaddr, int port) : base(localaddr, port)
    {
    }

    public override async Task HandleClientAsync(TcpClient client, ClientInfo info)
    {
        try
        {
            _ = info.Reader ?? throw new ArgumentNullException();
            _ = info.Writer ?? throw new ArgumentNullException();
            await info.Reader.BaseStream.CopyToAsync(info.Writer.BaseStream).ConfigureAwait(false);
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Socket Error: {e}");
        }
        finally
        {
            client.Close();
            Console.WriteLine($"Client disconnected: IP {info.Ip} Port {info.Port}");
        }
    }
}
