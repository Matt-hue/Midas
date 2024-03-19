using Midas.Data.Entities;

namespace Midas.Services
{
    public interface ICandleService
    {
        Task CreateRangeAsync(IEnumerable<Candle> entities);
    }
}