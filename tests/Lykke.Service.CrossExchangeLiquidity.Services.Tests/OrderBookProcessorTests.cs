using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
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

        private OrderBook CreateOrderBook()
        {
            return new OrderBook(Source, AssetPairId, new VolumePrice[0], new VolumePrice[0], DateTime.Now);
        }

        [Fact]
        public async Task Process_WhenFilterAccepts_OrderBookIsAddedToExchange()
        {
            var fabric = new OrderBookProcessorFabric();
            fabric.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<OrderBook>())).Returns(true);
            var orderBookProcessor = fabric.CreateOrderBookProcessor();
            OrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            Assert.True(fabric.CompositeExchange.ContainsKey(orderBook.AssetPairId));
        }

        [Fact]
        public async Task Process_WhenFilterDoesNotAccept_OrderBookIsNotAddedToExchange()
        {
            var fabric = new OrderBookProcessorFabric();
            fabric.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<OrderBook>())).Returns(false);
            var orderBookProcessor = fabric.CreateOrderBookProcessor();
            OrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            Assert.False(fabric.CompositeExchange.ContainsKey(orderBook.AssetPairId));
        }

        [Fact]
        public async Task Process_WhenFilterAccepts_OrdersArePlaced()
        {
            var fabric = new OrderBookProcessorFabric();
            fabric.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<OrderBook>())).Returns(true);
            var orderBookProcessor = fabric.CreateOrderBookProcessor();
            OrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            fabric.Trader.Verify(t=>t.PlaceOrdersAsync(It.IsAny<ICompositeOrderBook>()), Times.Once);
        }

        [Fact]
        public async Task Process_WhenFilterDoesNotAccept_OrdersAreNotPlaced()
        {
            var fabric = new OrderBookProcessorFabric();
            fabric.OrderBookFilter.Setup(f => f.IsAccepted(It.IsAny<OrderBook>())).Returns(false);
            var orderBookProcessor = fabric.CreateOrderBookProcessor();
            OrderBook orderBook = CreateOrderBook();

            await orderBookProcessor.ProcessAsync(orderBook);

            fabric.Trader.Verify(t => t.PlaceOrdersAsync(It.IsAny<ICompositeOrderBook>()), Times.Never);
        }
    }
}
