using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class VolumePartVolumePriceFilter : IVolumePriceFilter
    {
        private readonly IDictionary<string, decimal> _assetVolumeParts;
        private readonly IAssetPairDictionary _assetPairDictionary;

        public VolumePartVolumePriceFilter(IDictionary<string, decimal> assetVolumeParts,
                                        IAssetPairDictionary assetPairDictionary)
        {
            _assetVolumeParts = assetVolumeParts;
            _assetPairDictionary = assetPairDictionary;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            decimal volumePart = GetVolumePart(assetPairId);
            return asks.Select(p => new SourcedVolumePrice(p.Price, p.Volume * volumePart, p.Source));
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            decimal volumePart = GetVolumePart(assetPairId);
            return bids.Select(p => new SourcedVolumePrice(p.Price, p.Volume * volumePart, p.Source));
        }

        private decimal GetVolumePart(string assetPairId)
        {
            return _assetVolumeParts[_assetPairDictionary[assetPairId].BaseAssetId];
        }
    }
}
