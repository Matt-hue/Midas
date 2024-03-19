using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Midas.Services;
using Midas.Services.Workbook;
using System.IO;

namespace Midas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkbookController : ControllerBase
    {

        private readonly ILogger<WorkbookController> _logger;
        private readonly IReadExcelData _readExcelData;
        private readonly IMetricsWorkbookService _metricsWorkbookService;
        private readonly ICandleService _candleService;
        private readonly ITwelveDataCandleService _twelveDataCandleService;
        private readonly CondensedMetricsWorkbookService _condensedMetricsWorkbookService;

        public WorkbookController(
            ILogger<WorkbookController> logger,
            IReadExcelData readExcelData,
            IMetricsWorkbookService spreadSheetService,
            ICandleService candleService,
            ITwelveDataCandleService twelveDataCandleService,
            CondensedMetricsWorkbookService condensedMetricsWorkbookService)
        {
            _logger = logger;
            _readExcelData = readExcelData;
            _metricsWorkbookService = spreadSheetService;
            _candleService = candleService;
            _twelveDataCandleService = twelveDataCandleService;
            _condensedMetricsWorkbookService = condensedMetricsWorkbookService;
        }

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    var models = _readExcelData.ReadData();
        //    return Ok(models);
        //}

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var pairs = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("AEX", "DJI"),
                new Tuple<string, string>("NVDA", "AMD"),
                new Tuple<string, string>("LRCX", "AMAT"),

                new Tuple<string, string>("VTLE", "REI"),
                new Tuple<string, string>("VTLE", "FANG"),

                new Tuple<string, string>("CBRG", "EQH"),
                //new Tuple<string, string>("GS", "WFS"),
            };

            var l = new List<string>();
            foreach (var tuple in pairs)
            {
                var (first, second) = tuple;
                l.Add(first);
                l.Add(second);
            }
            var distinct = l.Distinct().ToList();

            foreach (var symbol in distinct)
            {
                await _twelveDataCandleService.SeedLatestHistoricalDataAsync(symbol);
            }

            var wb = await _metricsWorkbookService.GenerateWorkbookAsync(pairs);
            var stream = new MemoryStream();
            wb.SaveAs(stream);

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var date = DateOnly.FromDateTime(DateTime.Now).ToShortDateString();
            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return new FileStreamResult(stream, mimeType)
            {
                FileDownloadName = $"Metrics_{date}.xlsx"
            };
        }

        [HttpGet("condensed")]
        public async Task<IActionResult> GetCondensedMetrics()
        {
            var pairs = new List<Tuple<string, string>>()
            {
                //new Tuple<string, string>("AEX", "DJI"),
                //new Tuple<string, string>("NVDA", "AMD"),
                //new Tuple<string, string>("LRCX", "AMAT"),

                new Tuple<string, string>("VTLE", "REI"),
                new Tuple<string, string>("VTLE", "FANG"),

                ////new Tuple<string, string>("CBRG", "EQH"),
            };

            var l = new List<string>();
            foreach (var tuple in pairs)
            {
                var (first, second) = tuple;
                l.Add(first);
                l.Add(second);
            }
            var distinct = l.Distinct().ToList();

            foreach (var symbol in distinct)
            {
                await _twelveDataCandleService.SeedLatestHistoricalDataAsync(symbol);
            }

            var wb = await _condensedMetricsWorkbookService.GenerateWorkbookAsync(pairs);
            var stream = new MemoryStream();
            wb.SaveAs(stream);

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var date = DateOnly.FromDateTime(DateTime.Now).ToShortDateString();
            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return new FileStreamResult(stream, mimeType)
            {
                FileDownloadName = $"Condensed_Metrics_{date}.xlsx"
            };
        }

        [HttpPost("local/upload")]
        public async Task<IActionResult> Post(string symbolName, string? interval)
        {
            var path = "C:\\Users\\MatteoCatrini\\Downloads\\Candle.xlsx";
            var tzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var models = _readExcelData.ReadCandleData(symbolName, interval ?? "1day", tzi, path);
            await _candleService.CreateRangeAsync(models);

            return Ok();
        }
    }
}