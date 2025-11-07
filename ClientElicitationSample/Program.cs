

using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using static ModelContextProtocol.Protocol.ElicitRequestParams;


var (command, arguments) = GetCommandAndArguments(args);

var clientTransport = new StdioClientTransport(new()
{
    Name = "Demo Server",
    Command = command,
    Arguments = arguments,
});

McpClientOptions options = new()
{
    ClientInfo = new()
    {
        Name = "ElicitationClient",
        Version = "1.0.0"
    },
    Capabilities = new()
    {
        Elicitation = new()
        {
            ElicitationHandler = HandleElicitationAsync
        }
    }
};
await using var mcpClient = await McpClient.CreateAsync(clientTransport, options);

var tools = await mcpClient.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with tools: {tool.Name}");
}

var result = await mcpClient.CallToolAsync(
           "guess_the_number",
           default);


foreach (var block in result.Content)
{
    if (block is TextContentBlock textBlock)
    {
        Console.WriteLine(textBlock.Text);
    }
    else
    {
        Console.WriteLine($"Unknown block type: {block.GetType().Name}");
    }
}

async ValueTask<ElicitResult> HandleElicitationAsync(ElicitRequestParams? requestParams, CancellationToken cancellationToken)
{
    if (requestParams?.RequestedSchema.Properties is null)
    {
        return new ElicitResult();
    }

    if (requestParams?.Message is not null)
    {
        Console.WriteLine(requestParams.Message);
    }

    var content = new Dictionary<string, JsonElement>();

    foreach (var property in requestParams!.RequestedSchema.Properties)
    {
        if (property.Value is BooleanSchema booleanSchema)
        {
            Console.Write($"{booleanSchema}:");
            var clientIput = Console.ReadLine();
            bool parsedBool;

            if (bool.TryParse(clientIput, out parsedBool))
            {
                content[property.Key] = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(parsedBool));
            }
            else if (string.Equals(clientIput?.Trim(), "yes", StringComparison.OrdinalIgnoreCase))
            {
                content[property.Key] = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(true));
            }
            else if (string.Equals(clientIput?.Trim(), "no", StringComparison.OrdinalIgnoreCase))
            {
                content[property.Key] = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(false));
            }
            else if (property.Value is NumberSchema numberSchema)
            {
                Console.Write($"{numberSchema.Description}:");
                var input = Console.ReadLine();

                if (double.TryParse(input, out double number))
                {
                    content[property.Key] = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(number));
                }

            }
            else if (property.Value is StringSchema stringSchema)
            {
                Console.Write($"{stringSchema.Description}:");
                var input = Console.ReadLine();
                content[property.Key] = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(input));
            }

        }

    }
    
     return new ElicitResult
    {
        Action = "accept",
        Content = content
    };
}


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
