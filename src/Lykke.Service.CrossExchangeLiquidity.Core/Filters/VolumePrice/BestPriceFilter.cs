using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class BestPriceFilter : IVolumePriceFilter
    {
        private readonly IBestPriceEvaluator _bestPriceEvaluator;

        public BestPriceFilter(IBestPriceEvaluator bestPriceEvaluator)
        {
            _bestPriceEvaluator = bestPriceEvaluator;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            decimal minAsk = _bestPriceEvaluator.GetMinAsk(assetPairId);
            return asks.Select(p => new SourcedVolumePrice(p.Price < minAsk ? minAsk : p.Price, p.Volume, p.Source));
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            decimal maxBid = _bestPriceEvaluator.GetMaxBid(assetPairId);
            return bids.Select(p => new SourcedVolumePrice(p.Price > maxBid ? maxBid : p.Price, p.Volume, p.Source));
        }
    }
}
