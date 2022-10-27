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

    public async Task StartServerAsync()
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
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);
                writer.AutoFlush = true;
                ClientInfo info = GetClientInfo(client, stream, reader, writer);
                Console.WriteLine($"Client Connected: IP {info.Ip} Port {info.Port}");
                // Fire and forget
                _ = Task.Run(() => HandleClientAsync(client, info));
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


    private ClientInfo GetClientInfo(TcpClient client, NetworkStream stream, StreamReader reader, StreamWriter writer)
    {
        ClientInfo clientInfo = new();

        IPEndPoint? remoteEndpoint = client.Client.RemoteEndPoint as IPEndPoint;
        IPAddress clientIp = remoteEndpoint?.Address ?? IPAddress.None;
        int clientPort = remoteEndpoint?.Port ?? -1;
        clientInfo.SetIP(clientIp);
        clientInfo.Port = clientPort;


        clientInfo.Stream = stream;
        clientInfo.Reader = reader;
        clientInfo.Writer = writer;

        clientInfo.ServerForceDisconnect = false;

        return clientInfo;
    }

    public abstract Task HandleClientAsync(TcpClient client, ClientInfo info);
}
