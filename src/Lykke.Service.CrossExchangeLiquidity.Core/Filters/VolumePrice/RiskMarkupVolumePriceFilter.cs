using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class RiskMarkupVolumePriceFilter : IVolumePriceFilter
    {
        private readonly IDictionary<string, decimal> _assetRiskMarkup;
        private readonly IAssetPairDictionary _assetPairDictionary;

        public RiskMarkupVolumePriceFilter(IDictionary<string, decimal> assetRiskMarkup,
            IAssetPairDictionary assetPairDictionary)
        {
            _assetRiskMarkup = assetRiskMarkup;
            _assetPairDictionary = assetPairDictionary;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            decimal riskMarkup = GetRiskMarkup(assetPairId);
            return asks.Select(p => new SourcedVolumePrice(p.Price + riskMarkup, p.Volume, p.Source));
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            decimal riskMarkup = GetRiskMarkup(assetPairId);
            return bids.Select(p => new SourcedVolumePrice(p.Price - riskMarkup, p.Volume, p.Source));
        }

        public decimal GetRiskMarkup(string assetPairId)
        {
            return _assetRiskMarkup[_assetPairDictionary[assetPairId].QuotingAssetId];
        }
    }
}
