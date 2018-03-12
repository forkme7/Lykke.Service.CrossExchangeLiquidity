using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Extensions;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Moq;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests
{
    public class MatchingEngineTraderTests
    {
        private const string AssetPairId = "ETHBTC";

        private IOrderBook CreateOrderBook1()
        {
            return new Core.Domain.OrderBook.OrderBook(string.Empty,
                AssetPairId,
                new[]
                {
                    new VolumePrice(1, 1),
                    new VolumePrice(2, 1)
                },
                new VolumePrice[0],
                DateTime.Now);
        }

        private IOrderBook CreateOrderBook2()
        {
            return new Core.Domain.OrderBook.OrderBook(string.Empty,
                AssetPairId,
                new VolumePrice[0],
                new[]
                {
                    new VolumePrice(3, 1),
                    new VolumePrice(4, 1)
                },
                DateTime.Now);
        }

        [Fact]
        public async Task PlaceOrders_WhenModelIsChanged_OrdersArePlaced()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            IOrderBook orderBook1 = CreateOrderBook1();
            IOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrders(orderBook1);
            await Task.Delay(fabric.Settings.TimeSpan);
            await trader.PlaceOrders(orderBook2);

            fabric.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.IsAny<MultiLimitOrderModel>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task PlaceOrders_WhenTopFilterIsSet_TopOrdersArePlaced()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            IOrderBook orderBook1 = CreateOrderBook1();

            await trader.PlaceOrders(orderBook1);

            fabric.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.Is<MultiLimitOrderModel>(m=> m.Orders.Count == fabric.Settings.Filter.Count)),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenModelIsNotChanged_OrdersAreNotPlaced()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            IOrderBook orderBook = CreateOrderBook1();

            await trader.PlaceOrders(orderBook);
            await Task.Delay(fabric.Settings.TimeSpan);
            await trader.PlaceOrders(orderBook);

            fabric.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.IsAny<MultiLimitOrderModel>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenBreakIsNotOver_OrdersAreNotPlaced()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            IOrderBook orderBook1 = CreateOrderBook1();
            IOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrders(orderBook1);
            await trader.PlaceOrders(orderBook2);

            fabric.MatchingEngineClient.Verify(c => c.PlaceMultiLimitOrderAsync(It.IsAny<MultiLimitOrderModel>()),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenThereAreOnlySellOrders_CancelBuyOrders()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            IOrderBook orderBook2 = CreateOrderBook2();

            await trader.PlaceOrders(orderBook2);

            fabric.MatchingEngineClient.Verify(c => c.CancelMultiLimitOrderAsync(It.Is<MultiLimitOrderCancelModel>(m => m.IsBuy)),
                Times.Once);
        }

        [Fact]
        public async Task PlaceOrders_WhenThereAreOnlyBuyOrders_CancelSellOrders()
        {
            var fabric = new MatchingEngineTraderFabric();
            MatchingEngineTrader trader = fabric.CreateMatchingEngineTrader();
            IOrderBook orderBook1 = CreateOrderBook1();

            await trader.PlaceOrders(orderBook1);

            fabric.MatchingEngineClient.Verify(c => c.CancelMultiLimitOrderAsync(It.Is<MultiLimitOrderCancelModel>(m => !m.IsBuy)),
                Times.Once);
        }
    }
}
