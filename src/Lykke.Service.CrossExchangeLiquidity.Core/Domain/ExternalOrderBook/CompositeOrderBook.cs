using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook
{
    public class CompositeOrderBook : ICompositeOrderBook
    {
        public string AssetPairId { get; private set; }

        protected Dictionary<string, IExternalOrderBook> OrderBooks { get; private set; }

        public IEnumerable<SourcedVolumePrice> Asks => OrderBooks.Values
            .SelectMany(b => b.Asks.Select(p => new SourcedVolumePrice(p, b.Source)))
            .OrderBy(p => p.Price);

        public IEnumerable<SourcedVolumePrice> Bids => OrderBooks.Values
            .SelectMany(b => b.Bids.Select(p => new SourcedVolumePrice(p, b.Source)))
            .OrderByDescending(p => p.Price);

        public CompositeOrderBook(string assetPairId)
        {
            OrderBooks = new Dictionary<string, IExternalOrderBook>();
            AssetPairId = assetPairId;
        }

        public void AddOrUpdateOrderBook(IExternalOrderBook orderBook)
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
