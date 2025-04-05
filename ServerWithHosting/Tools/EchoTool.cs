using System.ComponentModel;
using ModelContextProtocol.Server;

namespace ServerWithHosting.Tools;

[McpServerToolType]
public sealed class EchoTool
{
  [McpServerTool, Description("Echoes the input back to the client.")]
  public static string Echo(string message)
  {
    return $"hello from host server: {message}";
  }
}