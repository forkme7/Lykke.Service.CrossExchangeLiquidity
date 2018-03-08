using Lykke.Service.CrossExchangeLiquidity.Core.Settings;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public class SourceAssetPairIdsFilter : IOrderBookFilter
    {
        private readonly SourceFilter _sourceFilter;
        private readonly AssetPairIdsFilter _assetPairIdsFilter;

        public SourceAssetPairIdsFilter(ISourceAssetPairIdsSettings settings)
        {
            _sourceFilter = new SourceFilter(settings);
            _assetPairIdsFilter = new AssetPairIdsFilter(settings);
        }

        public bool IsAccepted(Domain.OrderBook.OrderBook orderBook)
        {
            return _sourceFilter.IsAccepted(orderBook) && _assetPairIdsFilter.IsAccepted(orderBook);
        }
    }
}
