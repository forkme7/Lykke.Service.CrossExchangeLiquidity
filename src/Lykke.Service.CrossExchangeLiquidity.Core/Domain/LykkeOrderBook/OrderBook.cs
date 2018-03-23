using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook
{
    public class OrderBook
    {
        public string AssetPairId { get; }

        public IEnumerable<VolumePrice> Asks { get; set; }

        public IEnumerable<VolumePrice> Bids { get; set; }

        public OrderBook(string assetPairId)
        {
            AssetPairId = assetPairId;
        }
    }
}
