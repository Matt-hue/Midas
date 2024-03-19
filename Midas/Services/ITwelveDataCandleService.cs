using Midas.Models;

namespace Midas.Services
{
    public interface ITwelveDataCandleService
    {
        Task CreateRangeAsync(Dictionary<string, TwelveDataTimeSeriesDto> dict);
        Task<IEnumerable<CandleDto>> FindAsync(string? symbol, string? timeInterval = "1day", int size = 100);
        Task<CandleDto> GetByIdAsync(int id);
        Task SeedHistoricalDataAsync(string? symbols = null, int outputsize = 5000);
        Task SeedLatestHistoricalDataAsync(string symbol, string interval = "1day");
    }
}
