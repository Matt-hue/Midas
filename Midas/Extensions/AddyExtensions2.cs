using Midas.Models;
using Midas.Models.Algorithm;

namespace Midas.Extensions
{
    public static class AddyExtensions2
    {
        public static double? SetPurchasePriceRefPoint2(this LinkedListNode<CandlePair> node)
        {
            var currentNode = node.ValueRef;
            if (currentNode == null)
            {
                return null;
            }

            if (currentNode.PurchaseTrigger == 1)
            {
                return currentNode.FirstIndexClose;
            }
            else
            {
                return null;
            }
        }
        public static double? SetRefPrice2(this LinkedListNode<CandlePair> node)
        {
            double? refPrice;

            var previousNode = node.Previous?.ValueRef;
            if (previousNode == null)
            {
                return null;
            }

            bool previousSaleIsNotDone = previousNode.SaleDone != 1;
            bool maxWaitingPeriodNotExceeded = previousNode.WorkDayCounter + 1 <= MidasParameters.MaxWaitingPeriodInWorkdays;

            if (previousNode.RefPrice.HasValue && previousSaleIsNotDone && maxWaitingPeriodNotExceeded)
            {
                refPrice = previousNode.RefPrice.Value;
            }
            else if (previousNode.PurchasePriceRefPoint.HasValue)
            {
                refPrice = previousNode.PurchasePriceRefPoint.Value;
            }
            else
            {
                refPrice = null;
            }

            return refPrice;
        }
        public static int? SetPointsBelowTriggerInARow2(this LinkedListNode<CandlePair> node)
        {
            var currentNode = node.ValueRef;
            if (currentNode == null)
            {
                return null;
            }

            var previousNode = node?.Previous?.ValueRef;
            if (previousNode == null)
            {
                return null;
            }

            var belowAverage = currentNode.DeltaVsDynamicAverage < MidasParameters.RatioPercentageUnderDynamicAvg;

            if (belowAverage)
            {
                var saleNotDone = currentNode.SaleDone == null;
                var w = currentNode.WorkDayCounter != MidasParameters.MaxWaitingPeriodInWorkdays;


                if (previousNode.PointsBelowTrigger != null && saleNotDone && w)
                {
                    return previousNode.PointsBelowTrigger.Value + 1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return null;
            }
        }

        public static int? SetPurchaseTrigger2(this LinkedListNode<CandlePair> node)
        {
            var currentNode = node.ValueRef;
            if (currentNode == null)
            {
                return null;
            }

            if (currentNode.PointsBelowTrigger == MidasParameters.PointsInARowBelowAveragePriorToPurchase)
            {
                return 1;
            }

            return null;
        }

        public static int SetWorkdayCounter2(this LinkedListNode<CandlePair> node)
        {
            var currentNode = node.ValueRef;
            if (currentNode == null)
            {
                throw new NullReferenceException();
            }

            var previousNode = node?.Previous?.ValueRef;
            if (previousNode == null)
            {
                return 0;
            }

            if (currentNode.RefPrice != null && previousNode.SaleDone != 1 && currentNode.RefPrice == previousNode.RefPrice)
            {
                return previousNode.WorkDayCounter + 1;
            }
            else
            {
                if (currentNode.RefPrice.HasValue)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

        }

        public static int? SetPurchaseDone2(this LinkedListNode<CandlePair> node)
        {
            var currentNode = node.ValueRef;
            if (currentNode == null)
            {
                throw new NullReferenceException();
            }

            var previousNode = node?.Previous?.ValueRef;
            if (previousNode == null)
            {
                return null;
            }

            if (currentNode.WorkDayCounter == 1)
            {
                return 1;
            }
            else
            {
                return null;
            }

        }

        public static int? SetSaleDone2(this LinkedListNode<CandlePair> node)
        {
            var currentNode = node.ValueRef;
            if (currentNode == null)
            {
                throw new NullReferenceException();
            }

            if (currentNode.RefPrice.HasValue)
            {
                var shouldSetSaleAsDone = (currentNode.FirstIndexClose - currentNode.RefPrice) / currentNode.RefPrice >= MidasParameters.SalesPriceIncleaseVsPurchasePrice;
                if (shouldSetSaleAsDone)
                {
                    return 1;
                }
                else { return null; }
            }
            else
            {
                return null;
            }

        }

        public static double? SetDidNotMeetGoal2(this LinkedListNode<CandlePair> node)
        {
            var currentNode = node.ValueRef;
            if (currentNode == null)
            {
                throw new NullReferenceException();
            }

            if (currentNode.SaleDone != 1 && 
                currentNode.WorkDayCounter == MidasParameters.MaxWaitingPeriodInWorkdays)
            {
                var earnings = (currentNode.FirstIndexClose - currentNode.RefPrice) / currentNode.RefPrice;
                return earnings;
            }
            else
            {
                return null;
            }

        }

        public static double? SetMetGoalInTime2(this LinkedListNode<CandlePair> node)
        {
            var currentNode = node.ValueRef;
            if (currentNode == null)
            {
                throw new NullReferenceException();
            }

            if (currentNode.SaleDone == 1)
            {
                var earnings = (currentNode.FirstIndexClose - currentNode.RefPrice) / currentNode.RefPrice;
                return earnings;
            }
            else
            {
                return null;
            }

        }
    }
}
