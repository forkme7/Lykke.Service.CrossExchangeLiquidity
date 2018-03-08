using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public class CompositeOrderBook : ICompositeOrderBook
    {
        public string AssetPairId { get; private set; }

        protected Dictionary<string, IOrderBook> OrderBooks { get; private set; }

        public IEnumerable<VolumePrice> Asks => OrderBooks.Values.SelectMany(b=>b.Asks).OrderBy(p=>p.Price);

        public IEnumerable<VolumePrice> Bids => OrderBooks.Values.SelectMany(b => b.Bids).OrderBy(p => p.Price);

        public CompositeOrderBook(string assetPairId)
        {
            OrderBooks = new Dictionary<string, IOrderBook>();
            AssetPairId = assetPairId;
        }

        public void AddOrUpdateOrderBook(string source, IOrderBook orderBook)
        {
            if (!string.Equals(AssetPairId, orderBook.AssetPairId, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException(nameof(orderBook), $"{nameof(AssetPairId)} of orderbook is different from {nameof(AssetPairId)} of composite orderbook.");
            }

            OrderBooks[source] = orderBook;
        }
    }
}
