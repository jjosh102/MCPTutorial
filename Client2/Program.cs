

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using ModelContextProtocol.Client;

    


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


var builder = Kernel.CreateBuilder();

//qwen3:8b is good for tool calling
builder.AddOllamaChatCompletion("qwen3:8b", new Uri("http://localhost:11434"));

var kernel = builder.Build();

ChatCompletionAgent agent = new()
{
    Instructions =
    """
            You're an otaku who knows everything there is to know about anime.
            Provie only the best recommendations for anime based on the user's query.
            If you don't know the answer, say "I don't know" or "I don't have enough information to answer that".
            """,
    Name = "Anime Agent",
    Kernel = kernel,

    Arguments = new KernelArguments(new PromptExecutionSettings
    { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() })
};

kernel.Plugins.AddFromFunctions("Demo", tools.Select(tools => tools.AsKernelFunction()));

ChatHistoryAgentThread agentThread = new();
Console.WriteLine("Anime recommendations for Winter 2024:\n");
Console.WriteLine("User: What are the  anime of the winter 2024 season? Provide only the list in a nice format, no explanations or additional text.");
Console.Write("Anime Agent: ");

var thinkingIndicator = new[] { "|", "/", "-", "\\" };
int indicatorIndex = 0;
bool hasStarted = false;

await foreach (StreamingChatMessageContent response in agent.InvokeStreamingAsync("What are the best anime of the winter 2024 season?", agentThread))
{
    if (!string.IsNullOrWhiteSpace(response.Content))
    {
        if (!hasStarted)
        {
            
            Console.Write("\rAnime Agent: ");
            hasStarted = true;
        }
        Console.Write(response.Content);
    }
    else if (!hasStarted)
    {
    
        Console.Write($"\rAnime Agent: {thinkingIndicator[indicatorIndex++ % thinkingIndicator.Length]}");
        await Task.Delay(100);
    }
}

Console.WriteLine();
Console.WriteLine("\n--- End of recommendations ---");


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