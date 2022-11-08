
using Common.Library;
using Common.Library.Client;
using System.Net;
using System.Net.Sockets;

namespace BudgetChat;

class BudgetChatTcpServer : TcpServer
{
    private Handler _handler;

    public BudgetChatTcpServer(IPAddress localaddr, int port) : base(localaddr, port)
    {
        _handler = new Handler();
    }

    public override async Task HandleClientAsync(TcpClient client, ClientInfo info)
    {
        try
        {
            await _handler.ProcessRequestsAsync(info);
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Socket Error: {e}");
        }
        finally
        {
            if (!info.ServerForceDisconnect)
            {
                _handler.RemoveUserFromChatRoom(info);
                if (info.Name != null)
                {
                    await _handler.AnnounceLeaveMessageAsync(info.Name).ConfigureAwait(false); ;
                }
            }
            client.Close();
            Console.WriteLine($"Client disconnected: IP {info.Ip} Port {info.Port}");
        }
    }
}