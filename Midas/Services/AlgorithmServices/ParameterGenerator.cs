using MathNet.Numerics;
using Midas.Models.Algorithm;

namespace Midas.Services.AlgorithmServices
{
    public static class ParameterGenerator
    {
        public static IEnumerable<AddyParameters> Generate()
        {
            var parameters = new List<AddyParameters>();

            //var ratioPercentageUnderDynamicAvg = Enumerable.Range(1, 20).Select(x => (double)x/-1000);
            //var pointsInARowBelowAveragePriorToPurchase = Enumerable.Range(0, 4);
            //var workdaysThatDetermineDynamicAverage = Enumerable.Range(4, 15);
            //var salesPriceIncleaseVsPurchasePrice = Enumerable.Range(1, 20).Select(x => (double)x / 1000);
            //var maxWaitingPeriodInWorkdays = Enumerable.Range(4, 15);

            var ratioPercentageUnderDynamicAvg = Enumerable.Range(1, 16).Select(x => (double)x / -1000);
            var pointsInARowBelowAveragePriorToPurchase = Enumerable.Range(1, 3);
            var workdaysThatDetermineDynamicAverage = Enumerable.Range(4, 14);
            var salesPriceIncleaseVsPurchasePrice = Enumerable.Range(1, 28).Select(x => (double)x / 1000);
            var maxWaitingPeriodInWorkdays = Enumerable.Range(6, 14);

            foreach (var ratio in ratioPercentageUnderDynamicAvg)
            {
                foreach (var points in pointsInARowBelowAveragePriorToPurchase)
                {
                    foreach (var wd in workdaysThatDetermineDynamicAverage)
                    {
                        foreach (var price in salesPriceIncleaseVsPurchasePrice)
                        {
                            foreach (var wait in maxWaitingPeriodInWorkdays)
                            {
                                var p = new AddyParameters()
                                {
                                    RatioPercentageUnderDynamicAvg = ratio,
                                    PointsInARowBelowAveragePriorToPurchase = points,
                                    WorkdaysThatDetermineDynamicAverage = wd,
                                    SalesPriceIncleaseVsPurchasePrice = price,
                                    MaxWaitingPeriodInWorkdays = wait
                                };
                                parameters.Add(p);
                            }
                        }
                    }
                }
            }

            return parameters;
        }
    }
}
