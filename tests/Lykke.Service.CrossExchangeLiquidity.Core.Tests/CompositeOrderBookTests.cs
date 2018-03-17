using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
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

        private ExternalVolumePrice[] GetVolumePrices()
        {
            return new[]
            {
                new ExternalVolumePrice(1, 1),
                new ExternalVolumePrice(2, 1),
                new ExternalVolumePrice(3, 1),
                new ExternalVolumePrice(4, 1)
            };
        }

        [Fact]
        public void AddOrUpdateOrderBook_WhenAssetPairIdIsWrong_ExceptionIsThrown()
        {
            var compositeOrderBook = new CompositeOrderBook(AssetPairId);
            const string wrongAssetPairId = "BTCUSD";
            var orderBook = new ExternalOrderBook(Source1, 
                wrongAssetPairId, 
                new ExternalVolumePrice[0], 
                new ExternalVolumePrice[0],
                DateTime.Now);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                compositeOrderBook.AddOrUpdateOrderBook(orderBook));
        }

        [Fact]
        public void Asks_SortedByPriceAscending()
        {
            var compositeOrderBook = new CompositeOrderBook(AssetPairId);
            var volumePrices = GetVolumePrices().OrderBy(p => p.Price).ToArray();

            var orderBook1 = new ExternalOrderBook(Source1,
                AssetPairId,
                new []
                {
                    volumePrices[0],
                    volumePrices[2],
                },
                new ExternalVolumePrice[0],
                DateTime.Now);            

            var orderBook2 = new ExternalOrderBook(Source2,
                AssetPairId,
                new []
                {
                    volumePrices[1],
                    volumePrices[3],
                },
                new ExternalVolumePrice[0],
                DateTime.Now);

            compositeOrderBook.AddOrUpdateOrderBook(orderBook1);
            compositeOrderBook.AddOrUpdateOrderBook(orderBook2);

            var resultVolumePrices = compositeOrderBook.Asks.ToArray();
            volumePrices = volumePrices.OrderBy(p => p.Price).ToArray();
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
            var volumePrices = GetVolumePrices().OrderBy(p => p.Price).ToArray();

            var orderBook1 = new ExternalOrderBook(Source1,
                AssetPairId,
                new ExternalVolumePrice[0],
                new[]
                {
                    volumePrices[0],
                    volumePrices[2],
                },                
                DateTime.Now);            

            var orderBook2 = new ExternalOrderBook(Source2,
                AssetPairId,
                new ExternalVolumePrice[0],
                new[]
                {
                    volumePrices[1],
                    volumePrices[3],
                },
                DateTime.Now);

            compositeOrderBook.AddOrUpdateOrderBook(orderBook1);
            compositeOrderBook.AddOrUpdateOrderBook(orderBook2);

            var resultVolumePrices = compositeOrderBook.Bids.ToArray();
            volumePrices = volumePrices.OrderByDescending(p => p.Price).ToArray();
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
