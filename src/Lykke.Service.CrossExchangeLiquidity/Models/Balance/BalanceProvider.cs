using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.Balance;

namespace Lykke.Service.CrossExchangeLiquidity.Models.Balance
{
    public class BalanceProvider : IBalanceProvider
    {
        public IEnumerable<BalanceModel> Get(IDictionary<string, decimal> balances)
        {
            return balances?.Select(p => new BalanceModel()
            {
                AssetId = p.Key,
                Balance = p.Value
            });
        }

        public IEnumerable<BalanceModel> Get(string assetId, decimal balance)
        {
            return new[]
            {
                new BalanceModel()
                {
                    AssetId = assetId,
                    Balance = balance
                }
            };
        }

        public IEnumerable<SourceBalanceModel> Get(string source, string assetId, decimal balance)
        {
            return new[]
            {
                new SourceBalanceModel()
                {
                    Source = source,
                    Balances = new[]
                    {
                        new BalanceModel()
                        {
                            AssetId = assetId,
                            Balance = balance
                        }
                    }
                }
            };
        }

        public IEnumerable<SourceBalanceModel> Get(IDictionary<string, ReadOnlyDictionary<string, decimal>> balances)
        {
            return balances.Select(p => new SourceBalanceModel()
            {
                Source = p.Key,
                Balances = p.Value?.Select(a => new BalanceModel()
                {
                    AssetId = a.Key,
                    Balance = a.Value
                })
            });
        }

        public IEnumerable<SourceBalanceModel> Get(string source,
            IDictionary<string, ReadOnlyDictionary<string, decimal>> balances)
        {
            balances.TryGetValue(source, out var sourceBalances);
            return new[]
            {
                new SourceBalanceModel()
                {
                    Source = source,
                    Balances = sourceBalances?.Select(a => new BalanceModel()
                    {
                        AssetId = a.Key,
                        Balance = a.Value
                    })
                }
            };
        }

        public IEnumerable<SourceBalanceModel> GetByAsset(string assetId,
            IDictionary<string, ReadOnlyDictionary<string, decimal>> balances)
        {
            return balances.Select(p => new SourceBalanceModel()
            {
                Source = p.Key,
                Balances = p.Value?.Where(a => string.Equals(a.Key, assetId, StringComparison.OrdinalIgnoreCase))
                    .Select(a => new BalanceModel()
                    {
                        AssetId = a.Key,
                        Balance = a.Value
                    })
            });
        }
    }
}
