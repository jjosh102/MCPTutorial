using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using ServerWithHosting.Services;

namespace ServerWithHosting.Services;

[McpServerToolType]
public sealed class AnimeListTool
{
    [McpServerTool, Description("Get a list of anime by season and year.")]
    public static async Task<string> GetAnimeListBySeasonAndYear(
      AnimeListService animeListService,
      [Description("The season to get anime for: winter, fall, summer, spring.")]
      string season,
      [Description("The year to get anime for.")]
      int year)
    {
        var animeList = await animeListService.GetAnimeListBySeasonAndYear(season, year).ConfigureAwait(false);
        return JsonSerializer.Serialize(animeList);
    }

   
}

