using Microsoft.EntityFrameworkCore;
using Midas.Data;
using Midas.Data.Entities;
using Midas.Models;
using Midas.Models.Algorithm;
using Midas.Extensions;
using Midas.Models.Metrics;

namespace Midas.Services.AlgorithmServices
{
    public interface IParameterizedAddy
    {
        Task<IEnumerable<Tuple<AddyParameters, AddyCondensedMetrics>>> ExecuteAsync(string symbol1, string symbol2, IEnumerable<AddyParameters> parameterSet);
    }
    public class ParameterizedAddy : IParameterizedAddy
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParameterizedAddy(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Tuple<AddyParameters, AddyCondensedMetrics>>> ExecuteAsync(
            string symbol1, 
            string symbol2, 
            IEnumerable<AddyParameters> parameterSet
            )
        {

            var maxDateSymbol1 = await _unitOfWork.Candles
                .QueryableFind(x => x.Interval == "1day" && x.Symbol == symbol1)
                .MaxAsync(x => x.DateTimeLocal);

            var minDateSymbol1 = await _unitOfWork.Candles
                .QueryableFind(x => x.Interval == "1day" && x.Symbol == symbol1)
                .MinAsync(x => x.DateTimeLocal);

            var maxDateSymbol2 = await _unitOfWork.Candles
                .QueryableFind(x => x.Interval == "1day" && x.Symbol == symbol2)
                .MaxAsync(x => x.DateTimeLocal);

            var minDateSymbol2 = await _unitOfWork.Candles
                .QueryableFind(x => x.Interval == "1day" && x.Symbol == symbol2)
                .MinAsync(x => x.DateTimeLocal);

            var minDate = minDateSymbol1 >= minDateSymbol2 ? minDateSymbol1 : minDateSymbol2;
            var maxDate = maxDateSymbol1 <= maxDateSymbol2 ? maxDateSymbol1 : maxDateSymbol2;

            var groups = await _unitOfWork.Candles
                .QueryableFind(x =>
                x.Interval == "1day"
                && (x.Symbol == symbol1 || x.Symbol == symbol2)
                && (x.DateTimeLocal >= minDate && x.DateTimeLocal <= maxDate)
                )
                .GroupBy(x => x.DateTimeLocal)
                .ToListAsync();

            var condensedMetricsList0 = new List<AddyCondensedMetrics>();
            var condensedMetricsList = new List<Tuple<AddyParameters, AddyCondensedMetrics>>();
            foreach (var set in parameterSet)
            {
                var linkedList = new LinkedList<CandlePair>();

                foreach (IGrouping<DateTime, Candle> group in groups.OrderBy(x => x.Key))
                {
                    if (group.Count() < 2)
                    {
                        continue;
                    }

                    ApplyAddy(symbol1, symbol2, set, linkedList, group);
                }

                AddyCondensedMetrics condensedMetrics = linkedList.CalculateCondensedMetrics();
                var tuple = new Tuple<AddyParameters, AddyCondensedMetrics>(set, condensedMetrics);

                //condensedMetricsList0.Add(condensedMetrics);
                condensedMetricsList.Add(tuple);
            }

            

            return condensedMetricsList;
        }

        private static void ApplyAddy(
            string symbol1, 
            string symbol2,
            AddyParameters parameterSet,
            LinkedList<CandlePair> linkedList,
            IGrouping<DateTime, Candle> group)
        {

            var pair = new CandlePair(group.Key, group.First(x => x.Symbol.Equals(symbol1)), group.First(x => x.Symbol.Equals(symbol2)));
            var node = new LinkedListNode<CandlePair>(pair);
            linkedList.AddLast(node);

            pair.DynamicAverage = node.GetNodeMovingAverage2(parameterSet.WorkdaysThatDetermineDynamicAverage);

            pair.RefPrice = node.SetRefPrice3(parameterSet.MaxWaitingPeriodInWorkdays);
            pair.SaleDone = node.SetSaleDone3(parameterSet.SalesPriceIncleaseVsPurchasePrice);
            pair.WorkDayCounter = node.SetWorkdayCounter3();

            pair.PointsBelowTrigger = node.SetPointsBelowTriggerInARow3(parameterSet.RatioPercentageUnderDynamicAvg, parameterSet.MaxWaitingPeriodInWorkdays);
            pair.PurchaseTrigger = node.SetPurchaseTrigger3(parameterSet.PointsInARowBelowAveragePriorToPurchase);
            pair.PurchasePriceRefPoint = node.SetPurchasePriceRefPoint3();

            pair.PurchaseDone = node.SetPurchaseDone3();
            pair.MetGoalInTime = node.SetMetGoalInTime3();
            pair.DidNotMeetGoal = node.SetDidNotMeetGoal3(parameterSet.MaxWaitingPeriodInWorkdays);

        }
    }

}
