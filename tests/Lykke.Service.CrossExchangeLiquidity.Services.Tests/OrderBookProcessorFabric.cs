using Common.Log;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Services.OrderBook;
using Moq;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests
{
    public class OrderBookProcessorFabric
    {
        public Mock<ILog> Log { get; private set; }

        public Mock<IExternalOrderBookFilter> OrderBookFilter { get; private set; }

        public CompositeExchange CompositeExchange { get; private set; }

        public Mock<ILykkeTrader> Trader { get; private set; }

        public OrderBookProcessorFabric()
        {
            Log = new Mock<ILog>();
            OrderBookFilter = new Mock<IExternalOrderBookFilter>();
            CompositeExchange = new CompositeExchange();
            Trader = new Mock<ILykkeTrader>();
        }

        public ExternalOrderBookProcessor CreateOrderBookProcessor()
        {
            return new ExternalOrderBookProcessor(Log.Object,
                OrderBookFilter.Object,
                CompositeExchange,
                Trader.Object);
        }
    }
}
