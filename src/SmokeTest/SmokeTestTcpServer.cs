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

    public override async Task handleClient(TcpClient client, ClientInfo info)
    {
        try
        {
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream);
            using StreamWriter writer = new StreamWriter(stream);
            await reader.BaseStream.CopyToAsync(writer.BaseStream).ConfigureAwait(false);
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
