using Midas.Data.Entities;

namespace Midas.Data.Repositories
{
    public class CandleRepository : GenericRepository<Candle>, ICandleRepository
    {
        public CandleRepository(ApplicationDbContext context, ILogger logger)
            : base(context, logger)
        {
        }
    }
}
