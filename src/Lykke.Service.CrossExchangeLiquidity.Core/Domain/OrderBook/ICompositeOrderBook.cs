using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public interface ICompositeOrderBook
    {
        string AssetPairId { get; }

        IEnumerable<SourcedVolumePrice> Asks { get; }

        IEnumerable<SourcedVolumePrice> Bids { get; }

        void AddOrUpdateOrderBook(IOrderBook orderBook);
    }
}
