using System.Net;
using System.Net.Sockets;
using System.Text;

UdpClient listener = new UdpClient(8080);
Dictionary<string, byte[]> keyValuePairs = new();
byte[] versionMessage = Encoding.ASCII.GetBytes("version=Ken's Key-Value Store 1.0");
const string version = "version";
byte[] equal = { 61 };

try
{
    while (true)
    {
        Console.WriteLine("Waiting for broadcast");
        UdpReceiveResult udpReceiveResult = await listener.ReceiveAsync().ConfigureAwait(false);
        Console.WriteLine($"Received broadcast from {udpReceiveResult.RemoteEndPoint}");

        await ParseDataAndSendAsync(udpReceiveResult).ConfigureAwait(false);
    }
}
catch (SocketException e)
{
    Console.WriteLine(e);
}
finally
{
    listener.Close();
}


async Task ParseDataAndSendAsync(UdpReceiveResult udpReceiveResult)
{
    byte[] buffer = udpReceiveResult.Buffer;
    int index = Array.IndexOf(buffer, equal[0]);
    if (index == -1)
    {
        if (buffer.Length == 7 && Encoding.ASCII.GetString(buffer, 0, 7) == version)
        {
            await SendMessage(versionMessage, udpReceiveResult.RemoteEndPoint).ConfigureAwait(false);
            return;
        }
        string bufferString = Encoding.ASCII.GetString(buffer);
        if (keyValuePairs.ContainsKey(bufferString))
        {
            await SendMessage(buffer.Concat(equal).Concat(keyValuePairs[bufferString]).ToArray(), udpReceiveResult.RemoteEndPoint).ConfigureAwait(false);
            return;
        }
        await SendMessage(buffer.Concat(equal).ToArray(), udpReceiveResult.RemoteEndPoint).ConfigureAwait(false);
        return;
    }
    keyValuePairs[Encoding.ASCII.GetString(buffer.Take(index).ToArray())] = buffer.Skip(index + 1).ToArray();
}

async Task SendMessage(byte[] buffer, IPEndPoint remoteEndpoint)
{
    await listener.SendAsync(buffer, remoteEndpoint).ConfigureAwait(false);
}
