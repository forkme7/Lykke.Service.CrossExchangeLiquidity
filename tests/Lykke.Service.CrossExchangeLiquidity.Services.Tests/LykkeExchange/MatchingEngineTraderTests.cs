using System;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests.LykkeExchange
{
    public class MatchingEngineTraderTests
    {
        private const string Source = "bitfinex";
        private const string AssetPairId = "ETHBTC";

        private ICompositeOrderBook CreateOrderBook1()
        {
            var mock = new Mock<ICompositeOrderBook>();
            mock.Setup(b => b.AssetPairId).Returns(AssetPairId);
            mock.Setup(b => b.Asks).Returns(new[]
            {
                new SourcedVolumePrice(1, 1, Source),
                new SourcedVolumePrice(2, 1, Source)
            });
            mock.Setup(b => b.Bids).Returns(new SourcedVolumePrice[0]);
            return mock.Object;
        }

        private ICompositeOrderBook CreateOrderBook2()
        {
            var mock = new Mock<ICompositeOrderBook>();
            mock.Setup(b => b.AssetPairId).Returns(AssetPairId);
            mock.Setup(b => b.Asks).Returns(new SourcedVolumePrice[0]);
            mock.Setup(b => b.Bids).Returns(new[]
            {
                new SourcedVolumePrice(3, 1, Source),
                new SourcedVolumePrice(4, 1, Source)
            });
            return mock.Object;
        }

        [Fact]
        public async Task PlaceOrders_WhenModelIsChanged_OrdersArePlaced()
        {
            var factory = new MatchingEngineTraderFactory();
            MatchingEngineTrader trader = factory.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook1 = CreateOrderBook1();
            ICompositeOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrdersAsync(orderBook1);
            await Task.Delay(new TimeSpan(factory.Settings.Object.TimeSpan.Ticks + 100));
            await trader.PlaceOrdersAsync(orderBook2);

            factory.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.IsAny<MultiLimitOrderModel>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task PlaceOrders_WhenFilterIsSet_OrdersAreFiltered()
        {
            var factory = new MatchingEngineTraderFactory();            

            ICompositeOrderBook orderBook1 = CreateOrderBook1();
            factory.Filter.Setup(f => f.GetAsks(It.IsAny<string>(), It.IsAny<IEnumerable<SourcedVolumePrice>>()))
                .Returns(orderBook1.Asks);
            factory.Filter.Setup(f => f.GetBids(It.IsAny<string>(), It.IsAny<IEnumerable<SourcedVolumePrice>>()))
                .Returns(orderBook1.Bids);

            MatchingEngineTrader trader = factory.CreateMatchingEngineTrader();

            ICompositeOrderBook orderBook2 = CreateOrderBook2();
            
            await trader.PlaceOrdersAsync(orderBook2);

            factory.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.Is<MultiLimitOrderModel>(m=> 
                    m.Orders.All(o=>o.OrderAction == OrderAction.Buy? 
                        orderBook1.Bids.Any(b=>o.Price == (double)b.Price) 
                        : orderBook1.Asks.Any(b => o.Price == (double)b.Price))), 
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenModelIsNotChanged_OrdersAreNotPlaced()
        {
            var factory = new MatchingEngineTraderFactory();
            MatchingEngineTrader trader = factory.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook = CreateOrderBook1();

            await trader.PlaceOrdersAsync(orderBook);
            await Task.Delay(factory.Settings.Object.TimeSpan);
            await trader.PlaceOrdersAsync(orderBook);

            factory.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.IsAny<MultiLimitOrderModel>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenBreakIsNotOver_OrdersAreNotPlaced()
        {
            var factory = new MatchingEngineTraderFactory();
            MatchingEngineTrader trader = factory.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook1 = CreateOrderBook1();
            ICompositeOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrdersAsync(orderBook1);
            await trader.PlaceOrdersAsync(orderBook2);

            factory.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.IsAny<MultiLimitOrderModel>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenThereAreOnlySellOrders_CancelBuyOrders()
        {
            var factory = new MatchingEngineTraderFactory();
            MatchingEngineTrader trader = factory.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook1 = CreateOrderBook1();

            await trader.PlaceOrdersAsync(orderBook1);

            factory.MatchingEngineClient.Verify(c => c.CancelMultiLimitOrderAsync(It.Is<MultiLimitOrderCancelModel>(m => m.IsBuy), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenThereAreOnlyBuyOrders_CancelSellOrders()
        {
            var factory = new MatchingEngineTraderFactory();
            MatchingEngineTrader trader = factory.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrdersAsync(orderBook2);

            factory.MatchingEngineClient.Verify(c => c.CancelMultiLimitOrderAsync(It.Is<MultiLimitOrderCancelModel>(m => !m.IsBuy), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenMinAskIsProvided_PlaceOrderWithMinAsk()
        {
            var factory = new MatchingEngineTraderFactory();
            factory.BestPriceEvaluator.Setup(e => e.GetMinAsk(It.IsAny<string>())).Returns(decimal.MaxValue);

            MatchingEngineTrader trader = factory.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook1 = CreateOrderBook1();

            await trader.PlaceOrdersAsync(orderBook1);

            factory.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.Is<MultiLimitOrderModel>(m =>
                        m.Orders.All(o => o.Price == (double)decimal.MaxValue)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenMaxBidIsProvided_PlaceOrderWithMaxBid()
        {
            var factory = new MatchingEngineTraderFactory();
            factory.BestPriceEvaluator.Setup(e => e.GetMaxBid(It.IsAny<string>())).Returns(0);

            MatchingEngineTrader trader = factory.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrdersAsync(orderBook2);

            factory.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.Is<MultiLimitOrderModel>(m =>
                        m.Orders.All(o => o.Price == 0)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
