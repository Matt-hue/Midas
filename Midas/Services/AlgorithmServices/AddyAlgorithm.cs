using Microsoft.EntityFrameworkCore;
using Midas.Data;
using Midas.Data.Entities;
using Midas.Models;
using Midas.Models.Algorithm;
using Midas.Extensions;

namespace Midas.Services.AlgorithmServices
{
    public class AddyAlgorithm : IAddyAlgorithm
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddyAlgorithm(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CandlePair>> ExecuteAsync(string symbol1, string symbol2)
        {
            var linkedList = new LinkedList<CandlePair>();
            var groups = await _unitOfWork.Candles
                .QueryableFind(x => 
                x.Interval == "1day"
                && (x.Symbol == symbol1 || x.Symbol == symbol2)
                )
                .GroupBy(x => x.DateTimeLocal)
                .ToListAsync();

            foreach (IGrouping<DateTime, Candle> group in groups)
            {

                if (group.Count() < 2)
                    continue;

                var pair = new CandlePair(group.Key, group.First(x => x.Symbol.Equals(symbol1)), group.First(x => x.Symbol.Equals(symbol2)));
                var node = new LinkedListNode<CandlePair>(pair);
                linkedList.AddLast(node);

                pair.DynamicAverage = node.GetNodeMovingAverage(MidasParameters.WorkdaysThatDetermineDynamicAverage);
                pair.PointsBelowTrigger = node.SetPointsBelowTriggerInARow();
                pair.PurchaseTrigger = node.SetPurchaseTrigger();
                pair.PurchasePriceRefPoint = node.SetPurchasePriceRefPoint();
                pair.RefPrice = node.SetRefPrice();
                pair.WorkDayCounter = node.SetWorkdayCounter();
                pair.PurchaseDone = node.SetPurchaseDone();
                pair.SaleDone = node.SetSaleDone();
                pair.MetGoalInTime = node.SetMetGoalInTime();
            }

            return linkedList;
        }
    }
}
