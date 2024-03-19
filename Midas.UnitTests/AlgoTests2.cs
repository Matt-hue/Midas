using Midas.Extensions;
using Midas.Models.Algorithm;
using Midas.UnitTests.DataGenerator;
using Shouldly;

namespace Midas.UnitTests
{
    public class AlgoTests2
    {
        private LinkedList<CandlePair> _ll;
        public AlgoTests2()
        {
            _ll = CandlePairListGenerator.CreateLinkedList(3);
        }

        [Theory]
        [InlineData(611.06, 28634.88, 0.021329506, 1, 1, null, null)]
        [InlineData(612.87, 28868.8, 0.021340179, 6, 6, 1, 1)]
        public void SetPointsBelowTriggerInARow_sets_correct_points(
        double firstIndexClose,
        double secondIndexClose,
        double dynamicAverage,
        int? previousPointsBelowTriggerInARow,
        int workdayCounter,
        int? saleDone,
        int? expected
        )
        {
            //arrange
            LinkedListNode<CandlePair> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.FirstIndexClose= firstIndexClose;
            lastNode.ValueRef.SecondIndexClose = secondIndexClose;
            lastNode.ValueRef.DynamicAverage= dynamicAverage;
            lastNode.ValueRef.PointsBelowTrigger = previousPointsBelowTriggerInARow;
            lastNode.ValueRef.WorkDayCounter = workdayCounter;
            lastNode.ValueRef.SaleDone = saleDone;

            var previousNode = lastNode?.Previous?.ValueRef ?? throw new NullReferenceException();
            previousNode.PointsBelowTrigger = previousPointsBelowTriggerInARow;

            //act
            var result = lastNode.SetPointsBelowTriggerInARow2();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(1, 1)]
        [InlineData(6, null)]
        public void SetPurchaseTrigger_sets_correct_trigger(
        int? pointsBelowTriggerInARow,
        int? expected
        )
        {
            //arrange
            LinkedListNode<CandlePair> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.PointsBelowTrigger = pointsBelowTriggerInARow;

            //act
            var result = lastNode.SetPurchaseTrigger2();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(611.06, null, null)]
        [InlineData(612.87, 1, 612.87)]
        public void SetPurchasePriceRefPoint(
            double FirstIndexClose,
            int? purchaseTrigger, 
            double? expected
            )
        {
            //arrange
            LinkedListNode<CandlePair> lastNode = _ll.Last ?? throw new NullReferenceException();

            lastNode.ValueRef.FirstIndexClose = FirstIndexClose;
            lastNode.ValueRef.PurchaseTrigger = purchaseTrigger;

            //act
            var result = lastNode.SetPurchasePriceRefPoint2();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(612.87, 609.26, 6, 1, 612.87)]
        [InlineData(null, 609.26, 5, null, 609.26)]
        //[InlineData(604.58, 605.83, 1, null, 604.58)]
        public void SetRefPrice_sets_correct_price(
            double? previousPurchasePriceRefPoint,
            double previousRefPrice,
            int workdayCounter,
            int? saleDone,
            double expected
            )
        {
            //arrange
            LinkedListNode<CandlePair> lastNode = _ll.Last ?? throw new NullReferenceException();
            var previousNode = lastNode?.Previous?.ValueRef ?? throw new NullReferenceException();

            previousNode.PurchasePriceRefPoint = previousPurchasePriceRefPoint;
            previousNode.RefPrice = previousRefPrice;
            previousNode.WorkDayCounter = workdayCounter;
            previousNode.SaleDone = saleDone;

            //act
            var result = lastNode.SetRefPrice2();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(612.87, 609.26, 1, 6, 1)]
        [InlineData(609.26, 609.26, null, 5, 6)]
        [InlineData(null, null, null, 0, 0)]
        //[InlineData(605.77, null, null, 0, 1)]
        //[InlineData(null, null, null, 0, 0)]
        public void SetWorkdayCounter_sets_correct_counter(
            double? refPrice,
            double? previousRefPrice,
            int? previousSaleDone,
            int previousWorkdayCounter,
            int? expected
        )
        {
            //arrange
            LinkedListNode<CandlePair> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.RefPrice = refPrice;

            var previousNode = lastNode?.Previous?.ValueRef ?? throw new NullReferenceException();
            previousNode.RefPrice = previousRefPrice;
            previousNode.WorkDayCounter = previousWorkdayCounter;
            previousNode.SaleDone = previousSaleDone;

            //act
            int? result = lastNode.SetWorkdayCounter2();

            //assert
            result.ShouldBe(expected);
        }


        [Theory]
        [InlineData(0, null)]
        [InlineData(1, 1)]
        public void SetPurchaseDone_sets_correct_purchase_flag(
        int workdayCounter,
        int? expected
        )
        {
            //arrange
            LinkedListNode<CandlePair> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.WorkDayCounter = workdayCounter;

            //act
            var result = lastNode.SetPurchaseDone2();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(611.06, 612.87, null)]
        [InlineData(612.87, 604.58, 1)]
        [InlineData(604.58, 605.83, null)]
        [InlineData(605.77, null, null)]
        public void SetSaleDone_sets_correct_flag(
        double FirstIndexClose,
        double? refPrice,
        int? expected
        )
        {
            //arrange
            LinkedListNode<CandlePair> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.FirstIndexClose = FirstIndexClose;
            lastNode.ValueRef.RefPrice = refPrice;

            //act
            var result = lastNode.SetSaleDone2();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(611.06, 612.87, null, null)]
        [InlineData(612.87, 604.58, 1, 0.01371199841212075)]
        [InlineData(604.58, 605.83, null, null)]
        [InlineData(605.77, null, null, null)]
        public void SetMetGoalInTime_calculates_correct_earnings(
        double FirstIndexClose,
        double? refPrice,
        int? saleDone,
        double? expected
        )
        {
            //arrange
            LinkedListNode<CandlePair> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.FirstIndexClose = FirstIndexClose;
            lastNode.ValueRef.RefPrice = refPrice;
            lastNode.ValueRef.SaleDone = saleDone;

            //act
            var result = lastNode.SetMetGoalInTime2();

            //assert
            result.ShouldBe(expected);
        }
    }
}
