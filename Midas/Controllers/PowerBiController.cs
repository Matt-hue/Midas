using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Midas.Extensions;
using Midas.Models.Algorithm;
using Midas.Services;
using Midas.Services.AlgorithmServices;

namespace Midas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerBiController : ControllerBase
    {
        private readonly ILogger<PowerBiController> _logger;
        private readonly ITwelveDataCandleService _twelveDataCandleService;
        private readonly IAddyAlgorithm _addyAlgorithm;

        public PowerBiController(
            ILogger<PowerBiController> logger, 
            ITwelveDataCandleService twelveDataCandleService, 
            IAddyAlgorithm addyAlgorithm)
        {
            _logger = logger;
            _twelveDataCandleService = twelveDataCandleService;
            _addyAlgorithm = addyAlgorithm;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? symbol, [FromQuery] int? size )
        {
            var models = await _twelveDataCandleService.FindAsync(symbol: symbol, size: 2000);
            return Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var responseDto = await _twelveDataCandleService.GetByIdAsync(id);

            return Ok(responseDto);
        }

        [HttpGet("addy")]
        public async Task<IActionResult> ExecuteAddy(string? symbol1, string? symbol2)
        {
            //^AEX DJI
            //NVDA AMD
            var result = await _addyAlgorithm.ExecuteAsync(symbol1 ?? "NVDA", symbol2 ?? "AMD");
            result = result.Where(x => x.Datum < DateTime.Parse("04/01/2020"));
            return Ok(result.Reverse());
        }

        [HttpGet("addy/metrics")]
        public async Task<IActionResult> AddyMetrics(string? symbol1, string? symbol2)
        {
            IEnumerable<CandlePair> result = await _addyAlgorithm.ExecuteAsync(symbol1 ?? "^AEX2", symbol2 ?? "DJI2");
            //result = result.Where(x => x.Datum < DateTime.Parse("04/01/2020"));
            var metrics = result.CalculateMetrics();
            return Ok(metrics);
        }

    }
}
