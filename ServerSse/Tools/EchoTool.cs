using System.ComponentModel;
using ModelContextProtocol.Server;

namespace ServerSse.Tools;

[McpServerToolType]
public static class EchoTool
{
  [McpServerTool, Description("Echoes the input back to the client.")]
  public static string Echo(string message)
  {
    return $"hello from server: {message}";
  }
}