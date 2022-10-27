using BudgetChat;
using System.Net;

BudgetChatTcpServer server = new(IPAddress.Any, 8080);
await server.StartServerAsync();