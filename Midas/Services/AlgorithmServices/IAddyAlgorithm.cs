using Midas.Models.Algorithm;

namespace Midas.Services.AlgorithmServices
{
    public interface IAddyAlgorithm
    {
        Task<IEnumerable<CandlePair>> ExecuteAsync(string symbol1, string symbol2);
    }
}
