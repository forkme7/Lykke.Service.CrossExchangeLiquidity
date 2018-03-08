using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public class SourcesAssetPairIdsFilter : IOrderBookFilter
    {
        private readonly IEnumerable<SourceAssetPairIdsFilter> _filters;

        public SourcesAssetPairIdsFilter(IEnumerable<ISourceAssetPairIdsSettings> settings)
        {
            _filters = settings.Select(s => new SourceAssetPairIdsFilter(s));
        }

        public bool IsAccepted(Domain.OrderBook.OrderBook orderBook)
        {
            return _filters.Any(f => f.IsAccepted(orderBook));
        }
    }
}
