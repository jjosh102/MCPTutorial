using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;


await using var mcpClient = await McpClientFactory.CreateAsync(new()
{
    Id = "TestServerSse",
    Name = "TestServer",
    TransportType = TransportTypes.Sse,
    Location = "http://localhost:5021/sse",
});

var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with tools: {tool.Name}");
}
