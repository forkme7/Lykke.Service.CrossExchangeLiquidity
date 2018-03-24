using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Lykke.Service.CrossExchangeLiquidity.Models.OrderBook
{
    public class OrderBookProvider : IOrderBookProvider
    {
        private readonly IMapper _mapper;

        public OrderBookProvider(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<OrderBookModel> Get(string source, ICompositeOrderBook compositeOrderBook)
        {
            return new[]
            {
                new OrderBookModel()
                {
                    Source = source,
                    AssetPairId = compositeOrderBook.AssetPairId,
                    Asks = Get(source, compositeOrderBook.Asks),
                    Bids = Get(source, compositeOrderBook.Bids)
                }
            };
        }

        public IEnumerable<OrderBookModel> Get(ICompositeOrderBook compositeOrderBook)
        {
            IEnumerable<string> askSources = compositeOrderBook.Asks.Select(p => p.Source);
            IEnumerable<string> bidSources = compositeOrderBook.Bids.Select(p => p.Source);
            IEnumerable<string> sources = askSources.Union(bidSources).Distinct();

            return sources.Select(s => new OrderBookModel()
            {
                Source = s,
                AssetPairId = compositeOrderBook.AssetPairId,
                Asks = Get(s, compositeOrderBook.Asks),
                Bids = Get(s, compositeOrderBook.Bids)
            });
        }

        public IEnumerable<OrderBookModel> Get(IReadOnlyDictionary<string, ICompositeOrderBook> compositeExchange)
        {
            return compositeExchange.Values.SelectMany(Get);
        }

        public IEnumerable<OrderBookModel> Get(string source,
            IReadOnlyDictionary<string, ICompositeOrderBook> compositeExchange)
        {
            return compositeExchange.Values.SelectMany(b => Get(source, b));
        }

        private IEnumerable<VolumePriceModel> Get(string source, IEnumerable<SourcedVolumePrice> prices)
        {
            return prices.Where(p => string.Equals(p.Source, source, StringComparison.OrdinalIgnoreCase))
                .Select(p => new VolumePriceModel()
                {
                    Price = p.Price,
                    Volume = p.Volume
                });
        }

        public IEnumerable<OrderBookModel> Get(string source, IEnumerable<Core.Domain.LykkeOrderBook.OrderBook> orderBooks)
        {
            return orderBooks.Select(b => new OrderBookModel()
            {
                Source = source,
                AssetPairId = b.AssetPairId,
                Asks = _mapper.Map<IEnumerable<VolumePriceModel>>(b.Asks),
                Bids = _mapper.Map<IEnumerable<VolumePriceModel>>(b.Bids)
            });
        }

        public IEnumerable<OrderBookModel> Get(string source, Core.Domain.LykkeOrderBook.OrderBook orderBook)
        {
            return Get(source, new[] {orderBook});
        }
    }
}
