using Common.Library;
using Common.Library.Client;
using System.Net;
using System.Net.Sockets;

namespace PrimeTime;

class PrimeTimeTcpServer : TcpServer
{
    private Handler _handler;
    public PrimeTimeTcpServer(IPAddress localaddr, int port) : base(localaddr, port)
    {
        _handler = new Handler();
    }

    public override async Task handleClient(TcpClient client, ClientInfo info)
    {
        try
        {
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream);
            using StreamWriter writer = new StreamWriter(stream);
            await _handler.ProcessRequests(reader, writer);
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