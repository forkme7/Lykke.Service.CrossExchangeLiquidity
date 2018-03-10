using Common.Log;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Services.OrderBook;
using Moq;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests
{
    public class OrderBookProcessorFabric
    {
        public Mock<ILog> Log { get; private set; }

        public Mock<IOrderBookFilter> OrderBookFilter { get; private set; }

        public CompositeExchange CompositeExchange { get; private set; }

        public Mock<ITrader> Trader { get; private set; }

        public OrderBookProcessorFabric()
        {
            Log = new Mock<ILog>();
            OrderBookFilter = new Mock<IOrderBookFilter>();
            CompositeExchange = new CompositeExchange();
            Trader = new Mock<ITrader>();
        }

        public OrderBookProcessor CreateOrderBookProcessor()
        {
            return new OrderBookProcessor(Log.Object,
                OrderBookFilter.Object,
                CompositeExchange,
                Trader.Object);
        }
    }
}
