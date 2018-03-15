using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using System;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests
{
    public class CompositeExchangeTests
    {
        private const string Source = "bitfinex";
        private const string AssetPairId = "ETHBTC";

        [Fact]
        public void AddOrUpdateOrderBook_WhenAssetPairIdIsAdded_OrderBookIsCreated()
        {
            var compositeExchange = new CompositeExchange();
            var orderBook = new OrderBook(Source,
                AssetPairId,
                new VolumePrice[0],
                new VolumePrice[0],
                DateTime.Now);

            compositeExchange.AddOrUpdateOrderBook(orderBook);

            var resultOrderBook = compositeExchange[AssetPairId];
            Assert.NotNull(resultOrderBook);
        }
    }
}
