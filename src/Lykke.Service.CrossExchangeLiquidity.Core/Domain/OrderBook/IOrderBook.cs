using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public  interface IOrderBook
    {
        string AssetPairId { get; }

        IEnumerable<VolumePrice> Asks { get; }

        IEnumerable<VolumePrice> Bids { get; }
    }
}
