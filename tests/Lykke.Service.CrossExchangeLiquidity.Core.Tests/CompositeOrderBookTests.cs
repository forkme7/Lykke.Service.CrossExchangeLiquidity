using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using MoreLinq;
using System;
using System.Linq;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests
{
    public class CompositeOrderBookTests
    {
        private const string Source1 = "bitfinex";
        private const string Source2 = "otherex";
        private const string AssetPairId = "ETHBTC";

        private VolumePrice[] GetVolumePrices()
        {
            return new[]
            {
                new VolumePrice(1, 1),
                new VolumePrice(2, 1),
                new VolumePrice(3, 1),
                new VolumePrice(4, 1)
            };
        }

        [Fact]
        public void AddOrUpdateOrderBook_WhenAssetPairIdIsWrong_ExceptionIsThrown()
        {
            var compositeOrderBook = new CompositeOrderBook(AssetPairId);
            const string wrongAssetPairId = "BTCUSD";
            var orderBook = new OrderBook(Source1, 
                wrongAssetPairId, 
                new VolumePrice[0], 
                new VolumePrice[0],
                DateTime.Now);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                compositeOrderBook.AddOrUpdateOrderBook(orderBook));
        }

        [Fact]
        public void Asks_SortedByPriceAscending()
        {
            var compositeOrderBook = new CompositeOrderBook(AssetPairId);
            var volumePrices = GetVolumePrices().OrderBy(p => p.Price).ToArray();

            var orderBook1 = new OrderBook(Source1,
                AssetPairId,
                new []
                {
                    volumePrices[0],
                    volumePrices[2],
                },
                new VolumePrice[0],
                DateTime.Now);            

            var orderBook2 = new OrderBook(Source2,
                AssetPairId,
                new []
                {
                    volumePrices[1],
                    volumePrices[3],
                },
                new VolumePrice[0],
                DateTime.Now);

            compositeOrderBook.AddOrUpdateOrderBook(orderBook1);
            compositeOrderBook.AddOrUpdateOrderBook(orderBook2);

            var resultVolumePrices = compositeOrderBook.Asks.ToArray();
            for (int i = 0; i < resultVolumePrices.Length; i++)
            {
                var resultVolumePrice = resultVolumePrices[i];
                var volumePrice = volumePrices[i];
                Assert.Equal(resultVolumePrice.Price, volumePrice.Price);
                Assert.Equal(resultVolumePrice.Volume, volumePrice.Volume);
            }
        }

        [Fact]
        public void Bids_SortedByPriceDescending()
        {
            var compositeOrderBook = new CompositeOrderBook(AssetPairId);
            var volumePrices = GetVolumePrices().OrderByDescending(p => p.Price).ToArray();

            var orderBook1 = new OrderBook(Source1,
                AssetPairId,
                new VolumePrice[0],
                new[]
                {
                    volumePrices[0],
                    volumePrices[2],
                },                
                DateTime.Now);            

            var orderBook2 = new OrderBook(Source2,
                AssetPairId,
                new VolumePrice[0],
                new[]
                {
                    volumePrices[1],
                    volumePrices[3],
                },
                DateTime.Now);

            compositeOrderBook.AddOrUpdateOrderBook(orderBook1);
            compositeOrderBook.AddOrUpdateOrderBook(orderBook2);

            var resultVolumePrices = compositeOrderBook.Bids.ToArray();
            for (int i = 0; i < resultVolumePrices.Length; i++)
            {
                var resultVolumePrice = resultVolumePrices[i];
                var volumePrice = volumePrices[i];
                Assert.Equal(resultVolumePrice.Price, volumePrice.Price);
                Assert.Equal(resultVolumePrice.Volume, volumePrice.Volume);
            }
        }
    }
}
