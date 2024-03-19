using ClosedXML.Excel;
using Mapster;
using Midas.Extensions;
using Midas.Models.Metrics;
using Midas.Models.Workbook;
using Midas.Services.AlgorithmServices;

namespace Midas.Services.Workbook
{
    public interface IMetricsWorkbookService
    {
        Task<XLWorkbook> GenerateWorkbookAsync(IEnumerable<Tuple<string, string>> pairs);
    }
    public class MetricsWorkbookService : IMetricsWorkbookService
    {
        private readonly IAddyAlgorithm _addyAlgorithm;

        public MetricsWorkbookService(IAddyAlgorithm addyAlgorithm)
        {
            _addyAlgorithm = addyAlgorithm;
        }

        public async Task<XLWorkbook> GenerateWorkbookAsync(IEnumerable<Tuple<string, string>> pairs)
        {
            var wb = new XLWorkbook();

            var models = await GenerateModels(pairs);

            foreach (var tuple in models)
            {
                GenerateWorksheet(wb, tuple);
            }
            return wb;
        }

        private void GenerateWorksheet(XLWorkbook wb, Tuple<AddyMetricsSpreadsheetModel, AddyMetricsSpreadsheetModel> tuple)
        {
            IXLWorksheet ws = wb.AddWorksheet($"{tuple.Item1.Index1}-{tuple.Item2.Index1}");

            var tableTitleCell = ws.Cell(1, 1);
            tableTitleCell.SetValue($"{tuple.Item1.Index1} vs {tuple.Item1.Index2}");
            tableTitleCell.Style.Font.SetBold();

            var headerCell = ws.Cell(2, 2);
            headerCell.WorksheetRow().Style.Font.SetBold();
              //.Fill.SetBackgroundColor(XLColor.CornflowerBlue)
              //.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            headerCell.SetValue("Year");
            headerCell.CellRight(1).SetValue("Trade Count");
            headerCell.CellRight(2).SetValue("Trades that Met Goal");

            headerCell.CellRight(3).SetValue("Proceeds of Successful Trades");
            headerCell.CellRight(3).CellBelow().WorksheetColumn().Style.NumberFormat.NumberFormatId = 10;

            headerCell.CellRight(4).SetValue("Proceeds of Other Trades");
            headerCell.CellRight(5).SetValue("Total Proceeds");
            headerCell.CellRight(6).SetValue("Market Close at Year Start");
            headerCell.CellRight(7).SetValue("Market Performance");
            
            var table = headerCell.CellBelow().InsertData(tuple.Item1.Metrics);

            var inverseTableTitleCell = table.LastRowUsed().FirstCell().CellLeft().CellBelow(2);
            inverseTableTitleCell.Style.Font.SetBold();
            inverseTableTitleCell.SetValue($"{tuple.Item2.Index1} vs {tuple.Item2.Index2}");

            var inverseTableCell = inverseTableTitleCell.CellRight().CellBelow();
            var inverseTable = inverseTableCell.InsertData(tuple.Item2.Metrics);

            ws.Columns().AdjustToContents();
        }

        private async Task<IEnumerable<Tuple<AddyMetricsSpreadsheetModel, AddyMetricsSpreadsheetModel>>> GenerateModels(IEnumerable<Tuple<string, string>> pairs)
        {
            var models = new List<Tuple<AddyMetricsSpreadsheetModel, AddyMetricsSpreadsheetModel>>();
            foreach (var pair in pairs)
            {
                var output = await _addyAlgorithm.ExecuteAsync(pair.Item1, pair.Item2) ?? throw new ArgumentNullException();
                var metrics = output.CalculateMetrics() ?? new List<AddyMetrics>();
                var model = new AddyMetricsSpreadsheetModel()
                {
                    Index1 = pair.Item1,
                    Index2 = pair.Item2,
                    Metrics = metrics.Adapt<IEnumerable<AddySpreadsheetMetric>>()
                };

                var inverseMetrics = (await _addyAlgorithm.ExecuteAsync(pair.Item2, pair.Item1))?.CalculateMetrics() ?? new List<AddyMetrics>();
                var inverseModel = new AddyMetricsSpreadsheetModel()
                {
                    Index1 = pair.Item2,
                    Index2 = pair.Item1,
                    Metrics = inverseMetrics.Adapt<IEnumerable<AddySpreadsheetMetric>>()
                };

                var tuple = new Tuple<AddyMetricsSpreadsheetModel, AddyMetricsSpreadsheetModel>(model, inverseModel);

                models.Add(tuple);
            }

            return models;
        }
    }
}
