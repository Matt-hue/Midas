using Midas.Data.Repositories;

namespace Midas.Data
{
    public interface IUnitOfWork
    {
        public ICandleRepository Candles { get; }
        Task CompleteAsync();
    }
}
