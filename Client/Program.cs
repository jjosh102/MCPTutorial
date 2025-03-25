using ModelContextProtocol.Client;
using ModelContextProtocol.Configuration;
using ModelContextProtocol.Protocol.Transport;

var serverConfig = new McpServerConfig
{
    Id = "test-server",
    Name = "Test Server",
    TransportType = TransportTypes.StdIo,
    Location = "/path/to/server",
    TransportOptions = new Dictionary<string, string>
    {
        ["arguments"] = "--test arg",
        ["workingDirectory"] = "/working/dir"
    }
};
McpClientOptions _defaultOptions = new()
{
    ClientInfo = new() { Name = "TestClient", Version = "1.0.0" }
};

await using var client = await McpClientFactory.CreateAsync(
    serverConfig,
    _defaultOptions,
   null,
    cancellationToken: default);
