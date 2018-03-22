using Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.OrderBook;
using System;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests.Filters.ExternalOrderBook
{
    public class SourceAssetPairIdsOrderBookFilterTests
    {
        private const string Source1 = "bitfinex";
        private const string Source2 = "otherex";
        private const string AssetPairId1 = "ETHBTC";
        private const string AssetPairId2 = "USDBTC";
        private const string AssetPairId3 = "EURBTC";

        private SourceAssetPairIdsExternalOrderBookFilter GetFilter()
        {
            return new SourceAssetPairIdsExternalOrderBookFilter(
                new SourceAssetPairIdsSettings()
                {
                    Source = Source1,
                    AssetPairIds = new[] {AssetPairId1, AssetPairId2}
                });
        }

        private Core.Domain.ExternalOrderBook.ExternalOrderBook GetOrderBook(string source, string assetPairId)
        {
            return new Core.Domain.ExternalOrderBook.ExternalOrderBook(source, 
                assetPairId, 
                new Core.Domain.ExternalOrderBook.ExternalVolumePrice[0], 
                new Core.Domain.ExternalOrderBook.ExternalVolumePrice[0], 
                DateTime.Now);
        }

        [Fact]
        public void IsAccepted_WhenAssetPairIdsEqual_RetrunsTrue()
        {
            var filter = GetFilter();
            var orderBook = GetOrderBook(Source1, AssetPairId1);

            bool result = filter.IsAccepted(orderBook);

            Assert.True(result);
        }

        [Fact]
        public void IsAccepted_WhenAssetPairIdsDoNotEqual_RetrunsFalse()
        {
            var filter = GetFilter();
            var orderBook = GetOrderBook(Source1, AssetPairId3);

            bool result = filter.IsAccepted(orderBook);

            Assert.False(result);
        }

        [Fact]
        public void IsAccepted_WhenSourcesEqual_RetrunsTrue()
        {
            var filter = GetFilter();
            var orderBook = GetOrderBook(Source1, AssetPairId1);

            bool result = filter.IsAccepted(orderBook);

            Assert.True(result);
        }

        [Fact]
        public void IsAccepted_WhenSourcesDoNotEqual_RetrunsFalse()
        {
            var filter = GetFilter();
            var orderBook = GetOrderBook(Source2, AssetPairId1);

            bool result = filter.IsAccepted(orderBook);

            Assert.False(result);
        }
    }
}
