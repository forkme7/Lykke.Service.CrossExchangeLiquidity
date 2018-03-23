using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook
{
    public class LykkeExchange: ILykkeExchange
    {
        private readonly ConcurrentDictionary<string, OrderBook> _dictionary;
        private readonly IClientIdSettings _settings;
        public LykkeExchange(IClientIdSettings settings)
        {
            _settings = settings;
            _dictionary = new ConcurrentDictionary<string, OrderBook>();
        }

        public void AddOrUpdate(LykkeOrderBook lykkeOrderBook)
        {
            _dictionary.AddOrUpdate(lykkeOrderBook.AssetPairId,
                k => UpdateOrderBook(new OrderBook(k), lykkeOrderBook),
                (k, v) => UpdateOrderBook(v, lykkeOrderBook));
        }

        public OrderBook GetOrderBook(string assetPairId)
        {
            if (_dictionary.TryGetValue(assetPairId, out var orderBook))
            {
                return orderBook;
            }

            return null;
        }

        private OrderBook UpdateOrderBook(OrderBook orderBook, LykkeOrderBook lykkeOrderBook)
        {
            if (lykkeOrderBook.IsBuy)
            {
                orderBook.Bids = lykkeOrderBook.Prices.Where(p =>
                        !string.Equals(p.ClientId, _settings.ClientId, StringComparison.OrdinalIgnoreCase))
                    .Select(p => new VolumePrice(p.Price, p.Volume))
                    .OrderByDescending(p => p.Price);
            }
            else
            {
                orderBook.Asks = lykkeOrderBook.Prices.Where(p =>
                        !string.Equals(p.ClientId, _settings.ClientId, StringComparison.OrdinalIgnoreCase))
                    .Select(p => new VolumePrice(p.Price, p.Volume))
                    .OrderBy(p => p.Price);
            }

            return orderBook;
        }
    }
}
