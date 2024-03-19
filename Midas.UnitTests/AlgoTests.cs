using Midas.Models;
using Midas.Services;
using Midas.UnitTests.DataGenerator;
using Midas.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace Midas.UnitTests
{
    public class AlgoTests
    {
        private LinkedList<ExcelDataRow> _ll;
        public AlgoTests()
        {
            _ll = AexDjLinkedListGenerator.CreateLinkedList(3);
        }

        [Theory]
        //[InlineData(611.06, 28634.88, 0.021329506, 1, 1, null, null)]
        [InlineData(612.87, 28868.8, 0.021340179, 1, 1, 1, 1)]
        //[InlineData(606.04, 28239.28, 0.02147028, null, 0, null, null)]
        public void SetPointsBelowTriggerInARow_sets_correct_points(
        double aexClose,
        double djClose,
        double dynamicAverage,
        int? previousPointsBelowTriggerInARow,
        int workdayCounter,
        int? saleDone,
        int? expected
        )
        {
            //arrange
            LinkedListNode<ExcelDataRow> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.AexClose= aexClose;
            lastNode.ValueRef.DjClose= djClose;
            lastNode.ValueRef.DynamicAverage= dynamicAverage;
            lastNode.ValueRef.PointsBelowTrigger = previousPointsBelowTriggerInARow;
            lastNode.ValueRef.WorkDayCounter = workdayCounter;
            lastNode.ValueRef.SaleDone = saleDone;

            var previousNode = lastNode?.Previous?.ValueRef ?? throw new NullReferenceException();
            previousNode.PointsBelowTrigger = previousPointsBelowTriggerInARow;

            //act
            var result = lastNode.SetPointsBelowTriggerInARow();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(1, 1)]
        public void SetPurchaseTrigger_sets_correct_trigger(
        int? pointsBelowTriggerInARow,
        int? expected
        )
        {
            //arrange
            LinkedListNode<ExcelDataRow> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.PointsBelowTrigger = pointsBelowTriggerInARow;

            //act
            var result = lastNode.SetPurchaseTrigger();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(611.06, null, null)]
        [InlineData(612.87, 1, 612.87)]
        public void SetPurchasePriceRefPoint(
            double aexClose,
            int? purchaseTrigger, 
            double? expected
            )
        {
            //arrange
            LinkedListNode<ExcelDataRow> lastNode = _ll.Last ?? throw new NullReferenceException();

            lastNode.ValueRef.AexClose = aexClose;
            lastNode.ValueRef.PurchaseTrigger = purchaseTrigger;

            //act
            var result = lastNode.SetPurchasePriceRefPoint();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(612.87, 604.58, 1, 1, 612.87)]
        [InlineData(604.58, 605.83, 1, null, 604.58)]
        public void SetRefPrice_sets_correct_price(
            double purchasePriceRefPoint,
            double refPrice,
            int workdayCounter,
            int? saleDone,
            double expected
            )
        {
            //arrange
            LinkedListNode<ExcelDataRow> lastNode = _ll.Last ?? throw new NullReferenceException();
            var previousNode = lastNode?.Previous?.ValueRef ?? throw new NullReferenceException();

            previousNode.PurchasePriceRefPoint = purchasePriceRefPoint;
            previousNode.RefPrice = refPrice;
            previousNode.WorkDayCounter = workdayCounter;
            previousNode.SaleDone = saleDone;

            //act
            var result = lastNode.SetRefPrice();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(612.87, 604.58, 1, 1, 1)]
        [InlineData(604.58, 605.83, null, 1, 1)]
        [InlineData(605.77, null, null, 0, 1)]
        [InlineData(null, null, null, 0, 0)]
        public void SetWorkdayCounter_sets_correct_counter(
            double? refPrice,
            double? previousRefPrice,
            int? previousSaleDone,
            int previousWorkdayCounter,
            int expected
        )
        {
            //arrange
            LinkedListNode<ExcelDataRow> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.RefPrice = refPrice;

            var previousNode = lastNode?.Previous?.ValueRef ?? throw new NullReferenceException();
            previousNode.RefPrice = previousRefPrice;
            previousNode.WorkDayCounter = previousWorkdayCounter;
            previousNode.SaleDone = previousSaleDone;

            //act
            int? result = lastNode.SetWorkdayCounter();

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
            LinkedListNode<ExcelDataRow> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.WorkDayCounter = workdayCounter;

            //act
            var result = lastNode.SetPurchaseDone();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(611.06, 612.87, null)]
        [InlineData(612.87, 604.58, 1)]
        [InlineData(604.58, 605.83, null)]
        [InlineData(605.77, null, null)]
        public void SetSaleDone_sets_correct_flag(
        double aexClose,
        double? refPrice,
        int? expected
        )
        {
            //arrange
            LinkedListNode<ExcelDataRow> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.AexClose = aexClose;
            lastNode.ValueRef.RefPrice = refPrice;

            //act
            var result = lastNode.SetSaleDone();

            //assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(611.06, 612.87, null, null)]
        [InlineData(612.87, 604.58, 1, 0.01371199841212075)]
        [InlineData(604.58, 605.83, null, null)]
        [InlineData(605.77, null, null, null)]
        public void SetMetGoalInTime_calculates_correct_earnings(
        double aexClose,
        double? refPrice,
        int? saleDone,
        double? expected
        )
        {
            //arrange
            LinkedListNode<ExcelDataRow> lastNode = _ll.Last ?? throw new NullReferenceException();
            lastNode.ValueRef.AexClose = aexClose;
            lastNode.ValueRef.RefPrice = refPrice;
            lastNode.ValueRef.SaleDone = saleDone;

            //act
            var result = lastNode.SetMetGoalInTime();

            //assert
            result.ShouldBe(expected);
        }
    }
}
