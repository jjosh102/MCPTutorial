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



var (command, arguments) = GetCommandAndArguments(args);

var clientTransport = new StdioClientTransport(new()
{
    Name = "Demo Server",
    Command = command,
    Arguments = arguments,
});

await using var mcpClient = await McpClientFactory.CreateAsync(clientTransport);

var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with tools: {tool.Name}");
}

var result = await mcpClient.CallToolAsync(
           "GetAnimeListBySeasonAndYear",
           new Dictionary<string, object?>
           {
               ["season"] = "winter",
               ["year"] = 2024
           },
           default);

Console.WriteLine($"Result: {result.Content.First().Text}");

static (string command, string[] arguments) GetCommandAndArguments(string[] args)
{
    return args switch
    {
        [var script] when script.EndsWith(".py") => ("python", args),
        [var script] when script.EndsWith(".js") => ("node", args),
        [var script] when Directory.Exists(script) || (File.Exists(script) && script.EndsWith(".csproj")) => ("dotnet", ["run", "--project", script, "--no-build"]),
        _ => ("dotnet", ["run", "--project", "../ServerWithHosting", "--no-build"])
    };
}