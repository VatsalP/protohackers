using MeansToAnEnd;
using System.Net;

MeansEndTcpServer server = new MeansEndTcpServer(IPAddress.Any, 8080);
await server.StartServerAsync();