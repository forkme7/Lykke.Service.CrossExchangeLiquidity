using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public interface IVolumePriceFilter
    {
        IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks);

        IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids);
    }
}
