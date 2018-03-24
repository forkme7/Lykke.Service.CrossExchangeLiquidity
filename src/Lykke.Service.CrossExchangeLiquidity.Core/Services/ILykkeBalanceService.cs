using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface ILykkeBalanceService
    {
        ReadOnlyDictionary<string, decimal> GetBalances();

        decimal GetAssetBalance(string assetId);

        Task AddAssetAsync(string assetId, decimal value);
    }
}
