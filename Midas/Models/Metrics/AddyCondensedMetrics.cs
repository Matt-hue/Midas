using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Midas.Models.Metrics
{
    public class AddyCondensedMetrics
    {
        public int TradeCount { get; set; }
        public int TradesThatMetGoal { get; set; }
        public double ProceedsOfSuccessfulTrades { get; set; }
        public double ProceedsOfOtherTrades { get; set; }
        public double TotalProceeds { get; set; }
        public double MarketPerformance { get; set; }
        public double ProceedsVsMarket { get; set; }
    }
}
