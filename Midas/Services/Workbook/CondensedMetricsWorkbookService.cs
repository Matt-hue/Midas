using ClosedXML.Excel;
using Midas.Models.Algorithm;
using Midas.Models.Metrics;
using Midas.Models.Workbook;
using Midas.Services.AlgorithmServices;
using System.Globalization;

namespace Midas.Services.Workbook
{
    public class CondensedMetricsWorkbookService
    {
        private readonly IParameterizedAddy _parameterizedAddy;

        public CondensedMetricsWorkbookService(IParameterizedAddy parameterizedAddy)
        {
            _parameterizedAddy = parameterizedAddy;
        }

        public async Task<XLWorkbook> GenerateWorkbookAsync(IEnumerable<Tuple<string, string>> pairs)
        {
            var wb = new XLWorkbook();

            IEnumerable<AddyParameters> parameterSets = ParameterGenerator.Generate();

            //var l = new List<AddyParameters>() { new AddyParameters()
            //    {
            //     RatioPercentageUnderDynamicAvg  = -0.002, //-0.2%
            //     PointsInARowBelowAveragePriorToPurchase  = 1,
            //     WorkdaysThatDetermineDynamicAverage  = 10,
            //     SalesPriceIncleaseVsPurchasePrice  = 0.003,
            //     MaxWaitingPeriodInWorkdays  = 10,
            //    } 
            //};

            IXLWorksheet ws = wb.AddWorksheet($"BestParameters");
            ws.Cell(1, 2).SetValue("Ratio percentage under average");
            ws.Cell(1, 3).SetValue("Points in a row below average prior to purchase");
            ws.Cell(1, 4).SetValue("Workdays that determine dynamic average");
            ws.Cell(1, 5).SetValue("Sales price increase vs purchase price");
            ws.Cell(1, 6).SetValue("Max waiting period in workdays");
            ws.Cell(1, 8).SetValue("Total proceeds");

            foreach (var pair in pairs)
            {
                var nextAvailableCell = ws.LastRowUsed().Cell(1).CellBelow(2);
                var nextAvailableCellNumber = nextAvailableCell.Address.RowNumber;

                ws.Cell(nextAvailableCellNumber, 1).SetValue($"{pair.Item1}-{pair.Item2}");

                IEnumerable<Tuple<AddyParameters, AddyCondensedMetrics>> models = await _parameterizedAddy.ExecuteAsync(pair.Item1, pair.Item2, parameterSets);

                Tuple<AddyParameters, AddyCondensedMetrics> max = models.MaxBy(x => x.Item2.TotalProceeds) ?? throw new NullReferenceException();

                ws.Cell(nextAvailableCellNumber, 2).SetValue(max.Item1.RatioPercentageUnderDynamicAvg.ToString("p3", CultureInfo.InvariantCulture));
                ws.Cell(nextAvailableCellNumber, 3).SetValue(max.Item1.PointsInARowBelowAveragePriorToPurchase.ToString());
                ws.Cell(nextAvailableCellNumber, 4).SetValue(max.Item1.WorkdaysThatDetermineDynamicAverage.ToString());
                ws.Cell(nextAvailableCellNumber, 5).SetValue(max.Item1.SalesPriceIncleaseVsPurchasePrice.ToString("p3", CultureInfo.InvariantCulture));
                ws.Cell(nextAvailableCellNumber, 6).SetValue(max.Item1.MaxWaitingPeriodInWorkdays.ToString());

                ws.Cell(nextAvailableCellNumber, 8).SetValue(max.Item2.TotalProceeds.ToString("p3", CultureInfo.InvariantCulture));

                Console.WriteLine($"{pair.Item1}-{pair.Item2}-Compeleted-{DateTime.Now.ToShortTimeString()}");

                /////////////

                ws.Cell(nextAvailableCellNumber + 1, 1).SetValue($"{pair.Item2}-{pair.Item1}");

                IEnumerable<Tuple<AddyParameters, AddyCondensedMetrics>> inverseModel = await _parameterizedAddy.ExecuteAsync(pair.Item2, pair.Item1, parameterSets);

                Tuple<AddyParameters, AddyCondensedMetrics> inverseMax = inverseModel.MaxBy(x => x.Item2.TotalProceeds) ?? throw new NullReferenceException();

                ws.Cell(nextAvailableCellNumber + 1, 2).SetValue(inverseMax.Item1.RatioPercentageUnderDynamicAvg.ToString("p3", CultureInfo.InvariantCulture));
                ws.Cell(nextAvailableCellNumber + 1, 3).SetValue(inverseMax.Item1.PointsInARowBelowAveragePriorToPurchase.ToString());
                ws.Cell(nextAvailableCellNumber + 1, 4).SetValue(inverseMax.Item1.WorkdaysThatDetermineDynamicAverage.ToString());
                ws.Cell(nextAvailableCellNumber + 1, 5).SetValue(inverseMax.Item1.SalesPriceIncleaseVsPurchasePrice.ToString("p3", CultureInfo.InvariantCulture));
                ws.Cell(nextAvailableCellNumber + 1, 6).SetValue(inverseMax.Item1.MaxWaitingPeriodInWorkdays.ToString());

                ws.Cell(nextAvailableCellNumber + 1, 8).SetValue(inverseMax.Item2.TotalProceeds.ToString("p3", CultureInfo.InvariantCulture));

                Console.WriteLine($"{pair.Item2}-{pair.Item1}-Compeleted-{DateTime.Now.ToShortTimeString()}");
            }

            ws.Columns().AdjustToContents();

            return wb;
        }

    }
}
