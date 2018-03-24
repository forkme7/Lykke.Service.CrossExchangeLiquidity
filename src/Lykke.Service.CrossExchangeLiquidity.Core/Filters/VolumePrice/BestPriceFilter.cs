using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class BestPriceFilter : IVolumePriceFilter
    {
        public IBestPriceEvaluator BestPriceEvaluator { get; }

        public BestPriceFilter(IBestPriceEvaluator bestPriceEvaluator)
        {
            BestPriceEvaluator = bestPriceEvaluator;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            decimal minAsk = BestPriceEvaluator.GetMinAsk(assetPairId);
            return asks.Select(p => new SourcedVolumePrice(p.Price < minAsk ? minAsk : p.Price, p.Volume, p.Source));
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            decimal maxBid = BestPriceEvaluator.GetMaxBid(assetPairId);
            return bids.Select(p => new SourcedVolumePrice(p.Price > maxBid ? maxBid : p.Price, p.Volume, p.Source));
        }
    }
}
