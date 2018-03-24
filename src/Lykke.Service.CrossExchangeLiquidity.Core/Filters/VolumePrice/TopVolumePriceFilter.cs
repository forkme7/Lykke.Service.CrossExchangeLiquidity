﻿using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class TopVolumePriceFilter : IVolumePriceFilter
    {
        public int Count { get; }

        public TopVolumePriceFilter(int count)
        {
            Count = count;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            return asks.Take(Count);
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            return bids.Take(Count);
        }
    }
}
