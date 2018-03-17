using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;

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
                k => UpdateOrderBook(new OrderBook(), lykkeOrderBook),
                (k, v) => UpdateOrderBook(v, lykkeOrderBook));
        }

        public OrderBook GetOrderBook(string assetPairId)
        {
            return _dictionary[assetPairId];
        }

        private OrderBook UpdateOrderBook(OrderBook orderBook, LykkeOrderBook lykkeOrderBook)
        {
            if (lykkeOrderBook.IsBuy)
            {
                orderBook.Bids = lykkeOrderBook.Prices.Where(p =>
                        !string.Equals(p.ClientId, _settings.ClientId, StringComparison.OrdinalIgnoreCase))
                    .Select(p => new VolumePrice()
                    {
                        Price = p.Price,
                        Volume = p.Volume
                    })
                    .OrderByDescending(p => p.Price);
            }
            else
            {
                orderBook.Asks = lykkeOrderBook.Prices.Where(p =>
                        !string.Equals(p.ClientId, _settings.ClientId, StringComparison.OrdinalIgnoreCase))
                    .Select(p => new VolumePrice()
                    {
                        Price = p.Price,
                        Volume = p.Volume
                    })
                    .OrderBy(p => p.Price);
            }

            return orderBook;
        }
    }
}
