using ClosedXML.Excel;
using Midas.Data.Entities;
using Midas.Extensions;
using Midas.Models;

namespace Midas.Services.Workbook
{
    public class ReadExcelData : IReadExcelData
    {
        public IEnumerable<Candle> ReadCandleData(
            string symbol,
            string interval,
            TimeZoneInfo localTimeZoneInfo,
            string filepath)
        {
            var workbook = new XLWorkbook(filepath);

            var ws = workbook.Worksheet(1);
            var firstRowUsed = ws.FirstRowUsed().RowUsed();

            var firstPossibleAddress = ws.Row(firstRowUsed.RowNumber()).FirstCell().Address;
            var lastPossibleAddress = ws.LastCellUsed().Address;

            var dataRange = ws.Range(firstPossibleAddress, lastPossibleAddress).CreateTable();//.RangeUsed();

            var firstTable = ws.Tables.First();
            var firstTableRows = firstTable.DataRange.Rows();//.Take(100).Reverse();

            var candles = new List<Candle>();
            foreach (var row in firstTableRows)
            {
                var localDate = row.Field("date").GetDateTime();
                if (interval == "1day")
                {
                    localDate = DateOnly.FromDateTime(localDate).ToDateTime(TimeOnly.MinValue);
                }

                var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDate, localTimeZoneInfo);

                var open = row.Field("open").GetDouble();
                var close = row.Field("close").GetDouble();
                var high = row.Field("high").GetDouble();
                var low = row.Field("low").GetDouble();
                var volume = row.Field("volume").GetDouble();

                var candle = new Candle()
                {
                    Symbol = symbol,
                    Interval = interval,
                    DateTimeLocal = localDate,
                    TimeZoneLocal = localTimeZoneInfo.StandardName,
                    DateTimeUTC = utcDateTime,
                    Open = open,
                    Close = close,
                    High = high,
                    Low = low,
                    Volume = volume
                };

                candles.Add(candle);
            }

            return candles;
        }

        public IEnumerable<ExcelDataRow> ReadData()
        {
            //using (var workbook = new XLWorkbook("C:\\Users\\MatteoCatrini\\Downloads\\TradeData.xlsx"))
            //{
            //}

            var workbook = new XLWorkbook("C:\\Users\\MatteoCatrini\\Downloads\\TradeData.xlsx");

            var ws = workbook.Worksheet(1);
            var firstRowUsed = ws.FirstRowUsed().RowUsed();

            var firstPossibleAddress = ws.Row(firstRowUsed.RowNumber()).FirstCell().Address;
            var lastPossibleAddress = ws.LastCellUsed().Address;

            var dataRange = ws.Range(firstPossibleAddress, lastPossibleAddress).RangeUsed();

            var tables = ws.Tables;
            var firstTable = tables.First();

            var firstTableRows = firstTable.DataRange.Rows().Take(100).Reverse();

            var linkedList = new LinkedList<ExcelDataRow>();
            foreach (var row in firstTableRows)
            {
                var datum = row.Field("Datum").GetDateTime();

                var aexOpen = row.Field("AEX open").GetDouble();
                var aexClose = row.Field("AEX close").GetDouble();

                var djOpen = row.Field("DJ open").GetDouble();
                var djClose = row.Field("DJ close").GetDouble();

                var eurUsdOpen = row.Field("EUR-USD open").GetDouble();
                var eurUsdClose = row.Field("EUR-USD close").GetDouble();

                var dynamicRangeRow = row.Field("Dynamic range row").GetDouble();

                var rowModel = new ExcelDataRow()
                {
                    Datum = datum,
                    AexOpen = aexOpen,
                    AexClose = aexClose,

                    DjOpen = djOpen,
                    DjClose = djClose,

                    EurUsdOpen = eurUsdOpen,
                    EurUsdClose = eurUsdClose,

                    DynamicRangeRow = dynamicRangeRow
                };

                LinkedListNode<ExcelDataRow> node = linkedList.AddLast(rowModel);
                rowModel.DynamicAverage = node.GetNodeMovingAverage(MidasParameters.WorkdaysThatDetermineDynamicAverage);
                rowModel.PointsBelowTrigger = node.SetPointsBelowTriggerInARow();
                rowModel.PurchaseTrigger = node.SetPurchaseTrigger();
                rowModel.PurchasePriceRefPoint = node.SetPurchasePriceRefPoint();
                rowModel.RefPrice = node.SetRefPrice();
                rowModel.WorkDayCounter = node.SetWorkdayCounter();
                rowModel.PurchaseDone = node.SetPurchaseDone();
                rowModel.SaleDone = node.SetSaleDone();
                rowModel.MetGoalInTime = node.SetMetGoalInTime();
            }

            //IEnumerable<ExcelDataRow> updatedLl = linkedList.SetMovingAverage(MidasParameters.WorkdaysThatDetermineDynamicAverage);

            return linkedList.Reverse();

        }
    }
}
