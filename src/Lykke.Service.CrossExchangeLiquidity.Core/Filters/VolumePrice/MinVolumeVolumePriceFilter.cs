using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class MinVolumeVolumePriceFilter : IVolumePriceFilter
    {
        private readonly IDictionary<string, decimal> _assetMinVolumes;
        private readonly IAssetPairDictionary _assetPairDictionary;

        public MinVolumeVolumePriceFilter(IDictionary<string, decimal> assetMinVolumes,
            IAssetPairDictionary assetPairDictionary)
        {
            _assetMinVolumes = assetMinVolumes;
            _assetPairDictionary = assetPairDictionary;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            decimal minVolume = GetMinVolume(assetPairId);
            return asks.Where(p => p.Volume >= minVolume);
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            decimal minVolume = GetMinVolume(assetPairId);
            return bids.Where(p => p.Volume >= minVolume);
        }

        private decimal GetMinVolume(string assetPairId)
        {
            return _assetMinVolumes[_assetPairDictionary[assetPairId].BaseAssetId];
        }
    }
}
