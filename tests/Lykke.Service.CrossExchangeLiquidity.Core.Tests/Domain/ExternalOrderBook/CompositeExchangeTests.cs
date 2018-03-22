using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using System;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests.Domain.ExternalOrderBook
{
    public class CompositeExchangeTests
    {
        private const string Source = "bitfinex";
        private const string AssetPairId = "ETHBTC";

        [Fact]
        public void AddOrUpdateOrderBook_WhenAssetPairIdIsAdded_OrderBookIsCreated()
        {
            var compositeExchange = new CompositeExchange();
            var orderBook = new Core.Domain.ExternalOrderBook.ExternalOrderBook(Source,
                AssetPairId,
                new ExternalVolumePrice[0],
                new ExternalVolumePrice[0],
                DateTime.Now);

            compositeExchange.AddOrUpdateOrderBook(orderBook);

            var resultOrderBook = compositeExchange[AssetPairId];
            Assert.NotNull(resultOrderBook);
        }
    }
}
