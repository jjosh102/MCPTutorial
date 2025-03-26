using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;

McpClientOptions mcpClientOptions = new()
    { ClientInfo = new() { Name = "TestClient", Version = "1.0.0" } };


McpServerConfig mcpServerConfig = new()
{
    Id = "TestServerSse",
    Name = "TestServer",
    TransportType = TransportTypes.Sse,
    Location = "http://localhost:5021/sse",
};

var mcpClient = McpClientFactory.CreateAsync(mcpServerConfig, mcpClientOptions).GetAwaiter().GetResult();



await foreach (var tool in mcpClient.ListToolsAsync())
{
    Console.WriteLine($"{tool.Name} ({tool.Description})");
}