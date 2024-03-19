using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Midas.Services;

namespace Midas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedingController : ControllerBase
    {
        private readonly ILogger<SeedingController> _logger;
        private readonly ITwelveDataCandleService _twelveDataCandleService;

        public SeedingController(ILogger<SeedingController> logger, ITwelveDataCandleService twelveDataCandleService)
        {
            _logger = logger;
            _twelveDataCandleService = twelveDataCandleService;
        }

        //[HttpPost]
        //public async Task<IActionResult> SeeData()
        //{
        //    await _twelveDataCandleService.SeedHistoricalDataAsync();
        //    return Ok();
        //}

        [HttpPost("latest")]
        public async Task<IActionResult> PullLatestData(string symbol, string? interval)
        {
            // examples VTLE NVDA DJI

            // todo acknoledge number of items added
            await _twelveDataCandleService.SeedLatestHistoricalDataAsync(symbol, interval ?? "1day");
            return Ok();
        }
    }
}
