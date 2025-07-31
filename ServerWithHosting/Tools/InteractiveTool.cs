using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using ServerWithHosting.Services;
using static ModelContextProtocol.Protocol.ElicitRequestParams;

namespace ServerWithHosting.Services;

[McpServerToolType]
public sealed class InteractiveTool
{
  [McpServerTool, Description("A simple game where the user has to guess a number between 1 and 10.")]
  public async Task<string> GuessTheNumber(
  IMcpServer server,
  CancellationToken token
)
  {

    if (server.ClientCapabilities?.Elicitation is null)
    {
      return "Elicitation is not supported by the client.";
    }

    // First ask the user if they want to play
    var playSchema = new RequestSchema
    {
      Properties =
        {
            ["Answer"] = new BooleanSchema()
        }
    };

    var playResponse = await server.ElicitAsync(new ElicitRequestParams
    {
      Message = "Do you want to play a game?",
      RequestedSchema = playSchema
    }, token);

    // Check if user wants to play
    if (playResponse.Action != "yes" || playResponse.Content?["Answer"].ValueKind != JsonValueKind.True)
    {
      return "Maybe next time!";
    }

    var nameSchema = new RequestSchema
    {
      Properties =
        {
            ["Name"] = new StringSchema()
            {
              Description = "Name Of the player",
              MinLength = 2,
              MaxLength = 50
            }
        }
    };

    var nameResponse = await server.ElicitAsync(new ElicitRequestParams
    {
      Message = "What is your name?",
      RequestedSchema = nameSchema
    }, token);

    string playerName = nameResponse.Content?["Name"].GetString() ?? "Player";

    Random random = new Random();
    int numberToGuess = random.Next(1, 11);
    int attempts = 0;
    var message = "Guess the number between 1 and 10";

    while (true)
    {
      attempts++;
      var guessSchema = new RequestSchema
      {
        Properties =
          {
              ["Guess"] = new NumberSchema()
              {
                Description = "Your guess",
                Minimum = 1,
                Maximum = 10
              }
          }
      };

      var guessResponse = await server.ElicitAsync(new ElicitRequestParams
      {
        Message = message,
        RequestedSchema = guessSchema
      }, token);

      if (playResponse.Action != "accept")
      {
        return "Maybe next time!";
      }

      int guess = guessResponse.Content?["Guess"].GetInt32() ?? 0;

      if (guess == numberToGuess)
      {
        return $"{playerName}, you guessed the number {numberToGuess} in {attempts} attempts!";
      }
      else if (guess < numberToGuess)
      {
        message = "Too low! Try again.";
      }
      else
      {
        message = "Too high! Try again.";
      }

    }

  }

}

