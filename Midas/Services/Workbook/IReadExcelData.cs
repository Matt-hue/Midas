using Midas.Data.Entities;
using Midas.Models;

namespace Midas.Services.Workbook
{
    public interface IReadExcelData
    {
        IEnumerable<Candle> ReadCandleData(string symbol, string interval, TimeZoneInfo localTimeZoneInfo, string filepath);
        IEnumerable<ExcelDataRow> ReadData();
    }
}
