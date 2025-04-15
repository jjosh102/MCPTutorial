using XtractXcel;

namespace ServerWithHosting.Services;

public class TrendReader
{
  private readonly List<TrendData> _trendDataList;
  public TrendReader()
  {
    _trendDataList = new ExcelExtractor()
         .WithHeader(true)
         .WithWorksheetIndex(1)
         .FromFile(@"C:\repos\MCPTutorial\ServerWithHosting\Services\trending_PH.xlsx")
         .Extract<TrendData>();
    Console.WriteLine($"Loaded {_trendDataList.Count} trend data entries.");
  }

  public IEnumerable<TrendData> SearchByKeyword(string keyword)
  {
    return _trendDataList.Where(t =>
        (!string.IsNullOrEmpty(t.Trend) && t.Trend.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
        (!string.IsNullOrEmpty(t.TrendBreakdown) && t.TrendBreakdown.Contains(keyword, StringComparison.OrdinalIgnoreCase))
    );
  }


  public IEnumerable<TrendData> SearchByVolumeRange(int minVolume, int maxVolume)
  {
    return _trendDataList.Where(t =>
    {
      var volume = ParseSearchVolume(t.SearchVolume);
      return volume >= minVolume && volume <= maxVolume;
    });
  }

  public IEnumerable<TrendData> GetTopTrends(int topN = 10)
  {
    return _trendDataList
        .OrderByDescending(t => ParseSearchVolume(t.SearchVolume))
        .Take(topN);
  }

  private static int ParseSearchVolume(string? volumeText)
  {
    if (string.IsNullOrWhiteSpace(volumeText)) return 0;


    volumeText = volumeText.Replace("+", "").Trim().ToUpperInvariant();
    if (volumeText.EndsWith("K")) return int.Parse(volumeText[..^1]) * 1_000;
    if (volumeText.EndsWith("M")) return int.Parse(volumeText[..^1]) * 1_000_000;

    return int.TryParse(volumeText, out var result) ? result : 0;

  }
}

public class TrendData
{
  [ExcelColumn("Trends")]
  public string? Trend { get; set; }

  [ExcelColumn("Search volume")]
  public string? SearchVolume { get; set; }

  [ExcelColumn("Started")]
  public string? Started { get; set; }

  [ExcelColumn("Ended")]
  public string? Ended { get; set; }

  [ExcelColumn("Trend breakdown")]
  public string? TrendBreakdown { get; set; }

  [ExcelColumn("Explore link")]
  public string? ExploreLink { get; set; }
}
