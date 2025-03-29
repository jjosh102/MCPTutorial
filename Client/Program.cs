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

await using var mcpClient = await McpClientFactory.CreateAsync(mcpServerConfig, mcpClientOptions);


await foreach(  var message in mcpClient.EnumerateToolsAsync())
{
    Console.WriteLine($"Tools: {message}");
}
