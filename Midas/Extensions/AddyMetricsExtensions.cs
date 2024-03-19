using Midas.Models.Algorithm;
using Midas.Models.Metrics;

namespace Midas.Extensions
{
    public static class AddyMetricsExtensions
    {
        public static IEnumerable<AddyMetrics> CalculateMetrics(this IEnumerable<CandlePair> nodes)
        {
            var groups = nodes.GroupBy(x => x.Datum.Year).OrderByDescending(x => x.Key);

            double latestMarketClose = groups!.First()!.MaxBy(x => x.Datum)!.FirstIndexClose;

            List<AddyMetrics> metrics = new();
            foreach (var key in groups)
            {
                var firstIndexYearStartClose = key.MinBy(x => x.Datum)?.FirstIndexClose;// ?? throw new NullReferenceException();
                var metric = new AddyMetrics()
                {
                    Year = key.Key,
                    TradeCount = key.Count(x => x.PurchaseDone == 1),
                    TradesThatMetGoal = key.Count(x => x.MetGoalInTime != null),
                    ProceedsOfSuccessfulTrades = key.Sum(x => x.MetGoalInTime.GetValueOrDefault(0)).ToString("0%"),
                    ProceedsOfOtherTrades = key.Sum(x => x.DidNotMeetGoal.GetValueOrDefault(0)).ToString("0%"),
                    TotalProceeds = key.Sum(x => x.TotalProceeds).ToString("0%"),
                    FirstIndexYearStartClose = firstIndexYearStartClose,
                    FirstIndexCloseAtYearStart = firstIndexYearStartClose.GetValueOrDefault(0).ToString("#.##"),
                    MarketPerformance = CalculateMarketPerformance(ref latestMarketClose, firstIndexYearStartClose!.Value)
                };
                metrics.Add(metric);
            }

            return metrics;
        }

        public static AddyCondensedMetrics CalculateCondensedMetrics(this IEnumerable<CandlePair> nodes)
        {
            var latestClose = nodes.MaxBy(x => x.Datum)!.FirstIndexClose;
            var firstClose = nodes.MinBy(x => x.Datum)!.FirstIndexClose;

            var metrics = new AddyCondensedMetrics()
            {
                TradeCount = nodes.Count(x => x.PurchaseDone == 1),
                TradesThatMetGoal = nodes.Count(x => x.MetGoalInTime != null),
                ProceedsOfSuccessfulTrades = nodes.Sum(x => x.MetGoalInTime.GetValueOrDefault(0)),
                ProceedsOfOtherTrades = nodes.Sum(x => x.DidNotMeetGoal.GetValueOrDefault(0)),
                TotalProceeds = nodes.Sum(x => x.TotalProceeds),
                MarketPerformance = (latestClose / firstClose) - 1
            };

            return metrics;
        }

        private static string? CalculateMarketPerformance(ref double yearAfter, double indexCloseAtYearStart)
        {
            var result = ((yearAfter / indexCloseAtYearStart) - 1).ToString("0%");
            yearAfter = indexCloseAtYearStart;
            return result;
        }
    }
}
