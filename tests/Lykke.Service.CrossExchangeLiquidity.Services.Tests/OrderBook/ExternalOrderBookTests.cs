using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests.OrderBook
{
    public class ExternalOrderBookTests
    {
        private const string Source = "bitfinex";
        private const string AssetPairId = "ETHBTC";

        private ExternalOrderBook CreateOrderBook()
        {
            return new ExternalOrderBook(Source, AssetPairId, new ExternalVolumePrice[0], new ExternalVolumePrice[0], DateTime.Now);
        }

        [Fact]
        public async Task Process_WhenFilterAccepts_OrderBookIsAddedToExchange()
        {
            var factory = new ExternalOrderBookFactory();
            factory.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<ExternalOrderBook>())).Returns(true);
            var orderBookProcessor = factory.CreateOrderBookProcessor();
            ExternalOrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            Assert.True(factory.CompositeExchange.ContainsKey(orderBook.AssetPairId));
        }

        [Fact]
        public async Task Process_WhenFilterDoesNotAccept_OrderBookIsNotAddedToExchange()
        {
            var factory = new ExternalOrderBookFactory();
            factory.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<ExternalOrderBook>())).Returns(false);
            var orderBookProcessor = factory.CreateOrderBookProcessor();
            ExternalOrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            Assert.False(factory.CompositeExchange.ContainsKey(orderBook.AssetPairId));
        }

        [Fact]
        public async Task Process_WhenFilterAccepts_OrdersArePlaced()
        {
            var factory = new ExternalOrderBookFactory();
            factory.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<ExternalOrderBook>())).Returns(true);
            var orderBookProcessor = factory.CreateOrderBookProcessor();
            ExternalOrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            factory.Trader.Verify(t=>t.PlaceOrdersAsync(It.IsAny<ICompositeOrderBook>()), Times.Once);
        }

        [Fact]
        public async Task Process_WhenFilterDoesNotAccept_OrdersAreNotPlaced()
        {
            var factory = new ExternalOrderBookFactory();
            factory.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<ExternalOrderBook>())).Returns(false);
            var orderBookProcessor = factory.CreateOrderBookProcessor();
            ExternalOrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            factory.Trader.Verify(t => t.PlaceOrdersAsync(It.IsAny<ICompositeOrderBook>()), Times.Never);
        }
    }
}
