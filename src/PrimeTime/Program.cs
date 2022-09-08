using PrimeTime;
using System.Net;

PrimeTimeTcpServer server = new PrimeTimeTcpServer(IPAddress.Any, 8080);
await server.startServer();