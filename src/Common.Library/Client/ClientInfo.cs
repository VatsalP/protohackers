using System.Net;

using System.Net.Sockets;

namespace Common.Library.Client;

public class ClientInfo
{
    public IPAddress? Ip { get; set; }

    public int Port { get; set; }

    public bool ServerForceDisconnect { get; set; }

    public NetworkStream? Stream { get; set; }

    public StreamReader? Reader { get; set; }

    public StreamWriter? Writer { get; set; }

    public string? Name { get; set; }

    public int RequestCount { get; set; }

    public void SetIP(IPAddress ip)
    {
        this.Ip = ip;
    }

    public IPAddress GetIP()
    {
        return this.Ip ?? IPAddress.None;
    }
}
