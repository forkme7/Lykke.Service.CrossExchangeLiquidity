using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook
{
    public class OrderBook
    {
        public IEnumerable<VolumePrice> Asks { get; set; }

        public IEnumerable<VolumePrice> Bids { get; set; }
    }
}
