using Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeOrderBook;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests.Filters.LykkeOrderBook
{
    public class AssetPairIdsLykkeOrderBookFilterTests
    {
        private const string AssetPairId1 = "ETHBTC";
        private const string AssetPairId2 = "USDBTC";

        private AssetPairIdsLykkeOrderBookFilter GetAssetPairIdsLykkeOrderBookFilter()
        {
            return new AssetPairIdsLykkeOrderBookFilter(new []{AssetPairId1});
        }

        private Core.Domain.LykkeOrderBook.LykkeOrderBook GetOrderBook(string assetPairId1)
        {
            return new Core.Domain.LykkeOrderBook.LykkeOrderBook() {AssetPairId = assetPairId1};
        }

        [Fact]
        public void IsAccepted_WhenOrderAssetPairIsRight_RetrunsTrue()
        {
            var filter = GetAssetPairIdsLykkeOrderBookFilter();
            var orderBook = GetOrderBook(AssetPairId1);

            bool result = filter.IsAccepted(orderBook);

            Assert.True(result);
        }

        [Fact]
        public void IsAccepted_WhenOrderAssetPairIsWrong_RetrunsFalse()
        {
            var filter = GetAssetPairIdsLykkeOrderBookFilter();
            var orderBook = GetOrderBook(AssetPairId2);

            bool result = filter.IsAccepted(orderBook);

            Assert.False(result);
        }
    }
}
