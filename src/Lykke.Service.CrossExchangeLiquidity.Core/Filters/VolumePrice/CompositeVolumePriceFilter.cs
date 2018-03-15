using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class CompositeVolumePriceFilter : IVolumePriceFilter
    {
        private readonly IEnumerable<IVolumePriceFilter> _filters;

        public CompositeVolumePriceFilter(IEnumerable<IVolumePriceFilter> filters)
        {
            _filters = filters;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            var result = asks;
            foreach (IVolumePriceFilter filter in _filters)
            {
                result = filter.GetAsks(assetPairId, result);
            }
            return result;
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            var result = bids;
            foreach (IVolumePriceFilter filter in _filters)
            {
                result = filter.GetBids(assetPairId, result);
            }
            return result;
        }
    }
}
