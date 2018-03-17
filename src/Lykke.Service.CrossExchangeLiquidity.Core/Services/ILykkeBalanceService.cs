using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface ILykkeBalanceService
    {
        decimal GetAssetBalance(string assetId);

        Task AddAssetAsync(string assetId, decimal value);
    }
}
