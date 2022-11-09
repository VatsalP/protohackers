using MobIntheMiddle;
using System.Net;

MobInTheMiddleTcpServer server = new(IPAddress.Any, 8080);
await server.StartServerAsync();
