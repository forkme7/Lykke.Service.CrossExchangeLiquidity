using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests
{
    public class AssetPairIdsFilterTests
    {
        private const string Source = "bitfinex";
        private const string AssetPairId1 = "ETHBTC";
        private const string AssetPairId2 = "USDBTC";
        private const string AssetPairId3 = "EURBTC";

        private AssetPairIdsFilter GetFilter()
        {
            return new AssetPairIdsFilter(
                new AssetPairIdsSettings() {AssetPairIds = new[] {AssetPairId1, AssetPairId2}});
        }

        private OrderBook GetOrderBook(string assetPairId)
        {
            return new OrderBook(Source, assetPairId, new VolumePrice[0], new VolumePrice[0], DateTime.Now);
        }

        [Fact]
        public void IsAccepted_WhenAssetPairIdsEqual_RetrunsTrue()
        {
            AssetPairIdsFilter filter = GetFilter();
            OrderBook orderBook = GetOrderBook(AssetPairId1);

            bool result = filter.IsAccepted(orderBook);

            Assert.True(result);
        }

        [Fact]
        public void IsAccepted_WhenAssetPairIdsDoNotEqual_RetrunsFalse()
        {
            AssetPairIdsFilter filter = GetFilter();
            OrderBook orderBook = GetOrderBook(AssetPairId3);

            bool result = filter.IsAccepted(orderBook);

            Assert.False(result);
        }
    }
}
