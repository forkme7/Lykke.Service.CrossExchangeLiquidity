using System.Collections.Generic;
using System.Collections.ObjectModel;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.Balance;

namespace Lykke.Service.CrossExchangeLiquidity.Models.Balance
{
    public interface IBalanceProvider
    {
        IEnumerable<BalanceModel> Get(IDictionary<string, decimal> balances);
        IEnumerable<BalanceModel> Get(string assetId, decimal balance);
        IEnumerable<SourceBalanceModel> Get(string source, string assetId, decimal balance);

        IEnumerable<SourceBalanceModel> Get(
            IDictionary<string, ReadOnlyDictionary<string, decimal>> balances);

        IEnumerable<SourceBalanceModel> Get(string source,
            IDictionary<string, ReadOnlyDictionary<string, decimal>> balances);

        IEnumerable<SourceBalanceModel> GetByAsset(string assetId,
            IDictionary<string, ReadOnlyDictionary<string, decimal>> balances);
    }
}
