using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Services.Tests;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests
{
    public class OrderBookProcessorTests
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
            var fabric = new OrderBookProcessorFabric();
            fabric.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<ExternalOrderBook>())).Returns(true);
            var orderBookProcessor = fabric.CreateOrderBookProcessor();
            ExternalOrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            Assert.True(fabric.CompositeExchange.ContainsKey(orderBook.AssetPairId));
        }

        [Fact]
        public async Task Process_WhenFilterDoesNotAccept_OrderBookIsNotAddedToExchange()
        {
            var fabric = new OrderBookProcessorFabric();
            fabric.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<ExternalOrderBook>())).Returns(false);
            var orderBookProcessor = fabric.CreateOrderBookProcessor();
            ExternalOrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            Assert.False(fabric.CompositeExchange.ContainsKey(orderBook.AssetPairId));
        }

        [Fact]
        public async Task Process_WhenFilterAccepts_OrdersArePlaced()
        {
            var fabric = new OrderBookProcessorFabric();
            fabric.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<ExternalOrderBook>())).Returns(true);
            var orderBookProcessor = fabric.CreateOrderBookProcessor();
            ExternalOrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            fabric.Trader.Verify(t=>t.PlaceOrdersAsync(It.IsAny<ICompositeOrderBook>()), Times.Once);
        }

        [Fact]
        public async Task Process_WhenFilterDoesNotAccept_OrdersAreNotPlaced()
        {
            var fabric = new OrderBookProcessorFabric();
            fabric.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<ExternalOrderBook>())).Returns(false);
            var orderBookProcessor = fabric.CreateOrderBookProcessor();
            ExternalOrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            fabric.Trader.Verify(t => t.PlaceOrdersAsync(It.IsAny<ICompositeOrderBook>()), Times.Never);
        }
    }
}
