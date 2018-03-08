using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public class AssetPairIdsFilter : IOrderBookFilter
    {
        private  readonly IAssetPairIdsSettings _settings;

        public AssetPairIdsFilter(IAssetPairIdsSettings settings)
        {
            _settings = settings;
        }

        public bool IsAccepted(Domain.OrderBook.OrderBook orderBook)
        {
            return _settings.AssetPairIds.Any(p => string.Equals(p, orderBook.AssetPairId));
        }
    }
}
