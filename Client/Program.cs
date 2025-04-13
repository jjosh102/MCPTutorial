
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;



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

// var result = await mcpClient.CallToolAsync(
//            "GetAnimeListBySeasonAndYear",
//            new Dictionary<string, object?>
//            {
//                ["season"] = "winter",
//                ["year"] = 2024
//            },
//            default);

// Console.WriteLine($"Result: {result.Content.First().Text}");


// This model is not enough to give any coherent response
var ollamaChatClient = new OllamaChatClient(new Uri("http://localhost:11434"), "llama3.2:3b");

using var factory =
    LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.None));

var client = new ChatClientBuilder(ollamaChatClient)
    .UseLogging(factory)
    .UseFunctionInvocation()
    .Build();


IList<ChatMessage> messages =
[
   new(ChatRole.System, """
                        You're an otaku who knows everything there is to know about anime.
                        """),
    new(ChatRole.User, "What are the best anime of the winter 2024 season?"),
];

var response =
    await client.GetResponseAsync(
        messages,
        new ChatOptions
        {
            Tools = [.. tools]
        });

Console.WriteLine(response);

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