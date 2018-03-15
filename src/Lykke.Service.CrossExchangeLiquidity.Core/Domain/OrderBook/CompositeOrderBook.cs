using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public class CompositeOrderBook : ICompositeOrderBook
    {
        public string AssetPairId { get; private set; }

        protected Dictionary<string, IOrderBook> OrderBooks { get; private set; }

        public IEnumerable<SourcedVolumePrice> Asks => OrderBooks.Values
            .SelectMany(b => b.Asks.Select(a => new SourcedVolumePrice(a, b.Source)))
            .OrderBy(p => p.Price);

        public IEnumerable<SourcedVolumePrice> Bids => OrderBooks.Values
            .SelectMany(b => b.Bids.Select(a => new SourcedVolumePrice(a, b.Source)))
            .OrderBy(p => p.Price);

        public CompositeOrderBook(string assetPairId)
        {
            OrderBooks = new Dictionary<string, IOrderBook>();
            AssetPairId = assetPairId;
        }

        public void AddOrUpdateOrderBook(IOrderBook orderBook)
        {
            if (!string.Equals(AssetPairId, orderBook.AssetPairId, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException(nameof(orderBook), $"{nameof(AssetPairId)} of orderbook is different from {nameof(AssetPairId)} of composite orderbook.");
            }

            OrderBooks[orderBook.Source] = orderBook;
        }

        public override string ToString()
        {
            return GetType().Name + " " +
                   $"AssetPairId = {AssetPairId} " +
                   $"Top1Ask = {Asks.FirstOrDefault()} " +
                   $"Top1Bids = {Bids.FirstOrDefault()}";
        }
    }
}
