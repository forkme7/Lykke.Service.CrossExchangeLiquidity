using Lykke.Service.CrossExchangeLiquidity.Core.Settings.OrderBook;
using System;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook
{
    public class SourceAssetPairIdsExternalOrderBookFilter : IExternalOrderBookFilter
    {
        private readonly ISourceAssetPairIdsSettings _settings;

        public SourceAssetPairIdsExternalOrderBookFilter(ISourceAssetPairIdsSettings settings)
        {
            _settings = settings;            
        }

        public bool IsAccepted(Domain.ExternalOrderBook.IExternalOrderBook orderBook)
        {
            return string.Equals(orderBook.Source, _settings.Source, StringComparison.OrdinalIgnoreCase) 
                   && _settings.AssetPairIds.Any(p => string.Equals(p, orderBook.AssetPairId));
        }
    }
}
