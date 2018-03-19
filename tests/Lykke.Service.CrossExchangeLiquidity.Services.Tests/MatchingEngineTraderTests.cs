using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Extensions;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Moq;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests
{
    public class MatchingEngineTraderTests
    {
        private const string Source = "bitfinex";
        private const string AssetPairId = "ETHBTC";

        private ICompositeOrderBook CreateOrderBook1()
        {
            return new SimpleCompositeOrderBook()
            {
                AssetPairId = AssetPairId,
                Asks = new[]
                {
                    new SourcedVolumePrice(1, 1, Source),
                    new SourcedVolumePrice(2, 1, Source)
                },
                Bids = new SourcedVolumePrice[0]
            };
        }

        private ICompositeOrderBook CreateOrderBook2()
        {
            return new SimpleCompositeOrderBook()
            {
                AssetPairId = AssetPairId,                
                Asks = new SourcedVolumePrice[0],
                Bids = new[]
                {
                    new SourcedVolumePrice(3, 1, Source),
                    new SourcedVolumePrice(4, 1, Source)
                },
            };
        }

        [Fact]
        public async Task PlaceOrders_WhenModelIsChanged_OrdersArePlaced()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook1 = CreateOrderBook1();
            ICompositeOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrdersAsync(orderBook1);
            await Task.Delay(fabric.Settings.TimeSpan);
            await trader.PlaceOrdersAsync(orderBook2);

            fabric.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.IsAny<MultiLimitOrderModel>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task PlaceOrders_WhenTopFilterIsSet_TopOrdersArePlaced()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook1 = CreateOrderBook1();

            await trader.PlaceOrdersAsync(orderBook1);

            fabric.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.Is<MultiLimitOrderModel>(m=> m.Orders.Count == MatchingEngineTraderFabric.Count), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenModelIsNotChanged_OrdersAreNotPlaced()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook = CreateOrderBook1();

            await trader.PlaceOrdersAsync(orderBook);
            await Task.Delay(fabric.Settings.TimeSpan);
            await trader.PlaceOrdersAsync(orderBook);

            fabric.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.IsAny<MultiLimitOrderModel>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenBreakIsNotOver_OrdersAreNotPlaced()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook1 = CreateOrderBook1();
            ICompositeOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrdersAsync(orderBook1);
            await trader.PlaceOrdersAsync(orderBook2);

            fabric.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.IsAny<MultiLimitOrderModel>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenThereAreOnlySellOrders_CancelBuyOrders()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook1 = CreateOrderBook1();

            await trader.PlaceOrdersAsync(orderBook1);

            fabric.MatchingEngineClient.Verify(c => c.CancelMultiLimitOrderAsync(It.Is<MultiLimitOrderCancelModel>(m => m.IsBuy), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenThereAreOnlyBuyOrders_CancelSellOrders()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            ICompositeOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrdersAsync(orderBook2);

            fabric.MatchingEngineClient.Verify(c => c.CancelMultiLimitOrderAsync(It.Is<MultiLimitOrderCancelModel>(m => !m.IsBuy), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
