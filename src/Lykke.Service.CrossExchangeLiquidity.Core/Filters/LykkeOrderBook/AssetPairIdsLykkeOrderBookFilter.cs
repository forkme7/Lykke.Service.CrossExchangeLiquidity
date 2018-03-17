using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeOrderBook
{
    public class AssetPairIdsLykkeOrderBookFilter : ILykkeOrderBookFilter
    {
        private readonly IEnumerable<string> _assetPairIds;
        public AssetPairIdsLykkeOrderBookFilter(IEnumerable<string> assetPairIds)
        {
            _assetPairIds = assetPairIds;
        }

        public bool IsAccepted(Domain.LykkeOrderBook.LykkeOrderBook orderBook)
        {
            return _assetPairIds.Any(p => string.Equals(p, orderBook.AssetPairId));
        }
    }
}
