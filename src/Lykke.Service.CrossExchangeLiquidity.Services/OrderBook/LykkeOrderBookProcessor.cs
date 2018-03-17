using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;

namespace Lykke.Service.CrossExchangeLiquidity.Services.OrderBook
{
    public class LykkeOrderBookProcessor : IMessageProcessor<Core.Domain.LykkeOrderBook.LykkeOrderBook>
    {
        private readonly ILog _log;
        private readonly ILykkeOrderBookFilter _lykkeOrderBookFilter;
        private readonly ILykkeExchange _lykkeExchange;

        public LykkeOrderBookProcessor(ILog log,
            ILykkeOrderBookFilter lykkeOrderBookFilter,
            ILykkeExchange lykkeExchange)
        {
            _log = log;
            _lykkeOrderBookFilter = lykkeOrderBookFilter;
            _lykkeExchange = lykkeExchange;
        }

        public async Task ProcessAsync(Core.Domain.LykkeOrderBook.LykkeOrderBook orderBook)
        {
            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, $">> {orderBook}");

            if (!_lykkeOrderBookFilter.IsAccepted(orderBook))
            {
                await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name,
                    $"Lykke order book is skipped: {orderBook}");
                return;
            }

            _lykkeExchange.AddOrUpdate(orderBook);

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, "<<");
        }
    }
}
