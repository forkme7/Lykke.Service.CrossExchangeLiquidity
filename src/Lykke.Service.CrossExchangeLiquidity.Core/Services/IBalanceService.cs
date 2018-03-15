using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface IBalanceService
    {
        decimal GetAssetBalance(string assetId);

        Task AddAssetAsync(string assetId, decimal value);
    }
}
