using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class TopVolumePriceFilter : IVolumePriceFilter
    {
        private readonly ICountSettings _settings;

        public TopVolumePriceFilter(ICountSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            return asks.Take(_settings.Count);
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            return bids.Take(_settings.Count);
        }
    }
}
