using Midas.Models.Metrics;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Midas.Models.Workbook
{
    public class AddyMetricsSpreadsheetModel
    {
        public string Index1 { get; set; } = string.Empty;
        public string Index2 { get; set; } = string.Empty;

        public IEnumerable<AddySpreadsheetMetric> Metrics = new List<AddySpreadsheetMetric>();
    }

    public class AddySpreadsheetMetric
    {
        public int Year { get; set; }
        public int TradeCount { get; set; }
        public int TradesThatMetGoal { get; set; }
        public string? ProceedsOfSuccessfulTrades { get; set; }
        public string? ProceedsOfOtherTrades { get; set; }
        public string? TotalProceeds { get; set; }
        public string? FirstIndexCloseAtYearStart { get; set; }
        public string? MarketPerformance { get; set; }
    }

    public class AddySpreadsheetMetric2
    {
        public AddySpreadsheetMetric2(AddyMetrics model)
        {
            Year = model.Year;
            TradeCount = model.TradeCount;
            TradesThatMetGoal = model.TradesThatMetGoal;

            //double.TryParse(model?.ProceedsOfSuccessfulTrades, p);
            var proceedsOfSuccessfulTrades = double.Parse(model.ProceedsOfSuccessfulTrades != null ? model.ProceedsOfSuccessfulTrades.Substring(0, model.ProceedsOfSuccessfulTrades.IndexOf('%')) : "0");
            ProceedsOfSuccessfulTrades = proceedsOfSuccessfulTrades;
            //ProceedsOfOtherTrades = double.Parse(model.ProceedsOfOtherTrades);
            //TotalProceeds = double.Parse(model.TotalProceeds);
            //FirstIndexCloseAtYearStart = double.Parse(model.FirstIndexCloseAtYearStart);
            //MarketPerformance = double.Parse(model.MarketPerformance);
        }

        public int Year { get; set; }
        public int TradeCount { get; set; }
        public int TradesThatMetGoal { get; set; }
        public double ProceedsOfSuccessfulTrades { get; set; }
        public double ProceedsOfOtherTrades { get; set; }
        public double TotalProceeds { get; set; }
        public double FirstIndexCloseAtYearStart { get; set; }
        public double MarketPerformance { get; set; }
    }
}
