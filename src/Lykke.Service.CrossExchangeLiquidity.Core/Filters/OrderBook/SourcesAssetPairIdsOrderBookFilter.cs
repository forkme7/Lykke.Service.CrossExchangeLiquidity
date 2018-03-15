using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public class SourcesAssetPairIdsOrderBookFilter : IOrderBookFilter
    {
        private readonly IEnumerable<SourceAssetPairIdsOrderBookFilter> _filters;

        public SourcesAssetPairIdsOrderBookFilter(IEnumerable<ISourceAssetPairIdsSettings> settings)
        {
            _filters = settings.Select(s => new SourceAssetPairIdsOrderBookFilter(s));
        }

        public bool IsAccepted(Domain.OrderBook.IOrderBook orderBook)
        {
            return _filters.Any(f => f.IsAccepted(orderBook));
        }
    }
}
