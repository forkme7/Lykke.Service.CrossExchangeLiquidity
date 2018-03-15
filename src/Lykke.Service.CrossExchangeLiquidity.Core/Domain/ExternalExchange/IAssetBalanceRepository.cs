using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalExchange
{
    public interface IAssetBalanceRepository
    {
        Task<IAssetBalance> GetAsync(string source, string assetId);

        Task<IEnumerable<IAssetBalance>> GetAsync(string source);

        Task<IEnumerable<IAssetBalance>> GetAsync(IEnumerable<string> sources);

        Task InsertAsync(IAssetBalance assetBalance);

        Task InsertAsync(IEnumerable<IAssetBalance> assetBalances);

        Task ReplaceAsync(IAssetBalance assetBalance);

        Task InsertOrReplaceAsync(IAssetBalance assetBalance);        
    }
}
