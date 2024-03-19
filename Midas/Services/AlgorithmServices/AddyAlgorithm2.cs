using Microsoft.EntityFrameworkCore;
using Midas.Data;
using Midas.Data.Entities;
using Midas.Models;
using Midas.Models.Algorithm;
using Midas.Extensions;

namespace Midas.Services.AlgorithmServices
{
    public class AddyAlgorithm2 : IAddyAlgorithm
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddyAlgorithm2(IUnitOfWork unitOfWork)
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

            groups = groups.Where(x => x.Key < DateTime.Parse("04/01/2020") 
            && x.Key > DateTime.Parse("10/12/2009")).ToList();

            foreach (IGrouping<DateTime, Candle> group in groups.OrderBy(x => x.Key))
            {

                if (group.Count() < 2)
                    continue;

                var pair = new CandlePair(group.Key, group.First(x => x.Symbol.Equals(symbol1)), group.First(x => x.Symbol.Equals(symbol2)));
                var node = new LinkedListNode<CandlePair>(pair);
                linkedList.AddLast(node);

                pair.DynamicAverage = node.GetNodeMovingAverage(MidasParameters.WorkdaysThatDetermineDynamicAverage);

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

            return linkedList;
        }
    }
}
