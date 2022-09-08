using Common.Library.Client;
using System.Net;
using System.Net.Sockets;

namespace Common.Library;

public abstract class TcpServer
{
    private TcpListener? _server;

    protected TcpServer(IPAddress localaddr, int port)
    {
        if (localaddr == null)
        {
            throw new ArgumentException("Need an IPAddress");
        }
        try
        {
            _server = new TcpListener(localaddr, port);
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Error: {e}");
        }
    }

    public async Task startServer()
    {
        if (_server == null)
        {
            throw new ArgumentNullException("Server cannot be null");
        }
        try
        {
            _server.Start();
            Console.WriteLine($"{this.GetType().Name} started");
            while (true)
            {
                TcpClient client = await _server.AcceptTcpClientAsync();
                ClientInfo info = GetClientInfo(client);
                Console.WriteLine($"Client Connected: IP {info.Ip} Port {info.Port}");
                // Fire and forget
                _ = Task.Run(() => handleClient(client, info));
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Error: {e}");
        }
        finally
        {
            _server.Stop();
        }
    }


    private ClientInfo GetClientInfo(TcpClient client)
    {
        IPEndPoint remoteEndpoint = (IPEndPoint)client.Client.RemoteEndPoint ?? new IPEndPoint(0, 0);
        IPAddress clientIp = remoteEndpoint.Address;
        int clientPort = remoteEndpoint.Port;
        return new ClientInfo(clientIp, clientPort);
    }

    public abstract Task handleClient(TcpClient client, ClientInfo info);
}
