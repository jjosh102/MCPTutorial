using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;


// await using var mcpSseClient = await McpClientFactory.CreateAsync(new()
// {
//     Id = "TestServerSse",
//     Name = "TestServer",
//     TransportType = TransportTypes.Sse,
//     Location = "http://localhost:5021/sse",
// });

// var tools = await mcpSseClient.ListToolsAsync();
// foreach (var tool in tools)
// {
//     Console.WriteLine($"Connected to server with tools: {tool.Name}");
// }

// var result = await mcpSseClient.CallToolAsync(
//            "Echo",
//            new Dictionary<string, object?>
//            {
//                ["message"] = "Hello MCP!"
//            },
//            default);

// Console.WriteLine($"Result: {result.Content.First().Text}");



await using var mcpHostClient = await McpClientFactory.CreateAsync(new()
{
    Id = "TestHostServer",
    Name = "TestHostServer",
    TransportType = TransportTypes.StdIo,
    Location = @"C:\repos\MCPTutorial\ServerWithHosting\bin\Debug\net9.0\ServerWithHosting.exe"
});

var tools = await mcpHostClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with tools: {tool.Name}");
}

var result = await mcpHostClient.CallToolAsync(
           "GetAnimeListBySeasonAndYear",
           new Dictionary<string, object?>
           {
               ["season"] = "winter",
               ["year"] = 2024
           },
           default);

Console.WriteLine($"Result: {result.Content.First().Text}");