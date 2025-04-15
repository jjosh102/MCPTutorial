using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;


namespace ServerWithHosting.Services;

[McpServerToolType]
public sealed class TrendDataTool
{
    [McpServerTool, Description("Search trend data by keyword.")]
    public static string SearchByKeyword(
        TrendReader trendReader,
        [Description("Keyword to search within trend title or breakdown.")] string keyword)
    {
        var results = trendReader.SearchByKeyword(keyword);
        return JsonSerializer.Serialize(results);
    }


    [McpServerTool, Description("Search trend data by estimated search volume range.")]
    public static string SearchByVolumeRange(
        TrendReader trendReader,
        [Description("Minimum search volume. (e.g., 10000)")] int minVolume,
        [Description("Maximum search volume. (e.g., 500000)")] int maxVolume)
    {
        var results = trendReader.SearchByVolumeRange(minVolume, maxVolume);
        return JsonSerializer.Serialize(results);
    }

    [McpServerTool, Description("Get top  trending topics.")]
    public static string GetTopTrends(
    TrendReader trendReader,
    [Description("Number of top trends to return. Default is 10.")] int topN = 10)
    {
        var results = trendReader.GetTopTrends(topN);
        return JsonSerializer.Serialize(results);
    }
}
