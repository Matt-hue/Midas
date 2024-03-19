using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Midas.Models.Metrics
{
    public class AddyMetrics
    {
        public int Year { get; set; }
        public int TradeCount { get; set; }
        public int TradesThatMetGoal { get; set; }
        public string? ProceedsOfSuccessfulTrades { get; set; }
        public string? ProceedsOfOtherTrades { get; set; }
        public string? TotalProceeds { get; set; }

        //[XmlIgnoreAttribute]
        [JsonIgnore]
        public double? FirstIndexYearStartClose { get; set; }
        public string? FirstIndexCloseAtYearStart { get; set; }
        public string? MarketPerformance { get; set; }
    }
}
