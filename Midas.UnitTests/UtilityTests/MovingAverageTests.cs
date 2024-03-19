using DocumentFormat.OpenXml.Drawing;
using Midas.Extensions;
using Midas.Models;
using Midas.Services;
using Midas.Services.Workbook;
using Shouldly;
using System.Diagnostics;

namespace Midas.UnitTests.UtilityTests
{
    public class MovingAverageTests
    {
        private LinkedList<ExcelDataRow> _ll;

        public MovingAverageTests()
        {
            _ll = createLinkedList(20);
        }

        [Fact]
        public void GetMovingAverage_gets_correct_average()
        {
            //arrange
            var period = 10;
            var red = new ReadExcelData();
            var lastNode = _ll.Last;

            //act
            //var result = red.GetMovingAverage(lastNode, period);
            var result = lastNode?.GetNodeMovingAverage(period);

            //assert
            IEnumerable<double> averages = _ll.MovingAverage(x => x.CloseCloseRatio, period);
            var expected = averages.Last();

            result.ShouldBe(expected);
        }
        private LinkedList<ExcelDataRow> createLinkedList(int size)
        {

            var ll = new LinkedList<ExcelDataRow>();

            for (int i = 1;i < size + 1; i++)
            {
                var r = new ExcelDataRow()
                {
                    Id = i,
                    AexClose = i,
                    DjClose = 2,
                };
                ll.AddLast(r);
            }

            return ll;
        }
    }
}