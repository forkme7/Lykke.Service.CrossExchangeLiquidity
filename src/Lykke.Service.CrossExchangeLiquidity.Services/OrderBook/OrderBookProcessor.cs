using System;
using System.Collections.Generic;
using System.Text;
using Common.Log;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Domain = Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Services.OrderBook
{
    public class OrderBookProcessor : IOrderBookProcessor
    {
        private readonly ILog _log;

        public OrderBookProcessor(ILog log)
        {
            _log = log;
        }

        public void Process(Domain.OrderBook orderBook)
        {           
            _log.WriteInfoAsync(nameof(OrderBookProcessor), nameof(Process), "test");
        }
    }
}
