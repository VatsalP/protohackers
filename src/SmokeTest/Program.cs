using SmokeTest;
using System.Net;

SmokeTestTcpServer server = new SmokeTestTcpServer(IPAddress.Any, 8080);
await server.startServer();