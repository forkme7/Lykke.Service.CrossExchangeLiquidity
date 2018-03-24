using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface IExternalBalanceService
    {
        decimal GetAssetBalance(string source, string assetId);

        Task AddAssetAsync(string source, string assetId, decimal value);

        ReadOnlyDictionary<string, ReadOnlyDictionary<string, decimal>> GetBalances();
    }
}
