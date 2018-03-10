using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests
{
    public class SourceFilterTests
    {
        private const string Source1 = "bitfinex";
        private const string Source2 = "otherex";
        private const string AssetPairId = "ETHBTC";

        private SourceFilter GetFilter()
        {
            return new SourceFilter(new SourceSettings() { Source = Source1 });
        }

        private OrderBook GetOrderBook(string source)
        {
            return new OrderBook(source, AssetPairId, new VolumePrice[0], new VolumePrice[0], DateTime.Now);
        }

        [Fact]
        public void IsAccepted_WhenSourcesEqual_RetrunsTrue()
        {
            SourceFilter filter = GetFilter();
            OrderBook orderBook = GetOrderBook(Source1);

            bool result = filter.IsAccepted(orderBook);

            Assert.True(result);
        }

        [Fact]
        public void IsAccepted_WhenSourcesDoNotEqual_RetrunsFalse()
        {
            SourceFilter filter = GetFilter();
            OrderBook orderBook = GetOrderBook(Source2);

            bool result = filter.IsAccepted(orderBook);

            Assert.False(result);
        }
    }
}
