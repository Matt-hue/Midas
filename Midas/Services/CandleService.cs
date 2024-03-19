using Midas.Data;
using Midas.Data.Entities;
using Midas.Models;

namespace Midas.Services
{
    public class CandleService : ICandleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CandleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateRangeAsync(IEnumerable<Candle> entities)
        {
            await _unitOfWork.Candles.AddRangeAsync(entities);
            await _unitOfWork.CompleteAsync();
        }
    }
}
