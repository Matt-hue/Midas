using Microsoft.EntityFrameworkCore;
using Midas.Data;
using Midas.Data.Entities;
using Midas.Models;
using Midas.Models.Algorithm;
using Midas.Extensions;

namespace Midas.Services.AlgorithmServices
{
    /// <summary>
    /// NB: To mantain transparency, this algorithm
    /// does not try to improve or optmize the formulas provided from the Addy spreadsheet.
    /// </summary>
    public class AddyAlgorithm3 : IAddyAlgorithm
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddyAlgorithm3(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CandlePair>> ExecuteAsync(string symbol1, string symbol2)
        {
            var linkedList = new LinkedList<CandlePair>();

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

            foreach (IGrouping<DateTime, Candle> group in groups.OrderBy(x => x.Key))
            {
                var activeGroup = group;
                if (group.Count() < 2)
                {
                    var previousNode = linkedList.Last;
                    if(previousNode == null)
                    {
                        continue;
                    }

                    var symbol = group.First().Symbol;
                    if (symbol == symbol2)
                        continue;

                    activeGroup = RepairSingleCandleGroup(symbol1, symbol2, previousNode, group);

                }

                var pair = new CandlePair(activeGroup.Key, activeGroup.First(x => x.Symbol.Equals(symbol1)), activeGroup.First(x => x.Symbol.Equals(symbol2)));
                var node = new LinkedListNode<CandlePair>(pair);
                linkedList.AddLast(node);

                pair.DynamicAverage = node.GetNodeMovingAverage2(MidasParameters.WorkdaysThatDetermineDynamicAverage);

                pair.RefPrice = node.SetRefPrice2();
                pair.SaleDone = node.SetSaleDone2();
                pair.WorkDayCounter = node.SetWorkdayCounter2();

                pair.PointsBelowTrigger = node.SetPointsBelowTriggerInARow2();
                pair.PurchaseTrigger = node.SetPurchaseTrigger2();
                pair.PurchasePriceRefPoint = node.SetPurchasePriceRefPoint2();

                pair.PurchaseDone = node.SetPurchaseDone2();
                pair.MetGoalInTime = node.SetMetGoalInTime2();
                pair.DidNotMeetGoal = node.SetDidNotMeetGoal2();
            }

            return linkedList.ToList();
        }

        //todo optimize & unit test
        //should the data only be extended for one symbol?
        // should the first symbol stay fixed?
        private static IGrouping<DateTime, Candle> RepairSingleCandleGroup(
            string symbol1,
            string symbol2,
            LinkedListNode<CandlePair> previousNode,
            IGrouping<DateTime, Candle> group)
        {
            var candleInGroup = group.First();
            var interval = candleInGroup.Interval;
            var oppositeSymbol = candleInGroup.Symbol == symbol1 ? symbol2 : symbol1 ?? throw new ArgumentNullException();
            
            
            double oppositeSymbolPreviousClose;

            if(oppositeSymbol == symbol1)
            {
                oppositeSymbolPreviousClose = previousNode.ValueRef.FirstIndexClose;
            }
            else
            {
                oppositeSymbolPreviousClose = previousNode.ValueRef.SecondIndexClose;
            }

            Candle newCandle = new()
            {
                Symbol = oppositeSymbol,
                Interval = interval,
                DateTimeLocal = group.Key,
                Open = oppositeSymbolPreviousClose,
                Close = oppositeSymbolPreviousClose,
            };

            var newGroup = group.Append(newCandle).GroupBy(x => x.DateTimeLocal).First();

            return newGroup;
        }

        //todo optimize & unit test
        private static IGrouping<DateTime, Candle>? AddMissingSecondaryCandle(
            string symbol1,
            string symbol2,
            LinkedListNode<CandlePair> previousNode,
            IGrouping<DateTime, Candle> group)
        {
            var candleInGroup = group.First();

            if(candleInGroup.Symbol == symbol2)
            {
                // ignore and continue
                return null;
            }

            var interval = candleInGroup.Interval;
            var oppositeSymbol = candleInGroup.Symbol == symbol1 ? symbol2 : symbol1 ?? throw new ArgumentNullException();


            double oppositeSymbolPreviousClose;

            if (oppositeSymbol == symbol1)
            {
                oppositeSymbolPreviousClose = previousNode.ValueRef.FirstIndexClose;
            }
            else
            {
                oppositeSymbolPreviousClose = previousNode.ValueRef.SecondIndexClose;
            }

            Candle newCandle = new()
            {
                Symbol = oppositeSymbol,
                Interval = interval,
                DateTimeLocal = group.Key,
                Open = oppositeSymbolPreviousClose,
                Close = oppositeSymbolPreviousClose,
            };

            var newGroup = group.Append(newCandle).GroupBy(x => x.DateTimeLocal).First();

            return newGroup;
        }

    }

}
