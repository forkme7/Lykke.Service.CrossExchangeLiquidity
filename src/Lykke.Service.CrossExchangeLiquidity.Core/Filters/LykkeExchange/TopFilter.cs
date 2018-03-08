using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeExchange
{
    public class TopFilter : ITradeFilter
    {
        private readonly ICountSettings _settings;

        public TopFilter(ICountSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<VolumePrice> GetAsks(IOrderBook orderBook)
        {
            return orderBook.Asks.TakeLast(_settings.Count);
        }

        public IEnumerable<VolumePrice> GetBids(IOrderBook orderBook)
        {
            return orderBook.Bids.Take(_settings.Count);
        }
    }
}
