using System.Threading.Tasks;
using Autofac;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface ILykkeBalanceService
    {
        decimal GetAssetBalance(string assetId);

        Task AddAssetAsync(string assetId, decimal value);
    }
}
