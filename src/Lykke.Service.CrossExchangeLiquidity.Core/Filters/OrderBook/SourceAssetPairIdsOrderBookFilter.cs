using Lykke.Service.CrossExchangeLiquidity.Core.Settings;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public class SourceAssetPairIdsOrderBookFilter : IOrderBookFilter
    {
        private readonly SourceOrderBookFilter _sourceFilter;
        private readonly AssetPairIdsOrderBookFilter _assetPairIdsFilter;

        public SourceAssetPairIdsOrderBookFilter(ISourceAssetPairIdsSettings settings)
        {
            _sourceFilter = new SourceOrderBookFilter(settings);
            _assetPairIdsFilter = new AssetPairIdsOrderBookFilter(settings);
        }

        public bool IsAccepted(Domain.OrderBook.IOrderBook orderBook)
        {
            return _sourceFilter.IsAccepted(orderBook) && _assetPairIdsFilter.IsAccepted(orderBook);
        }
    }
}
