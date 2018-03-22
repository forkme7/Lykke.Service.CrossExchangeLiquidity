using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Services.OrderBook;
using Moq;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests.OrderBook
{
    public class BestPriceEvaluatorTests
    {
        private const string AssetPairId = "ETHBTC";
        private const string QuotingAssetId = "BTC";
        private const string Source = "bitfinex";
        private const decimal MinHalfSpread = 1;


        private Core.Domain.LykkeOrderBook.OrderBook GetOrderBook()
        {
            return new Core.Domain.LykkeOrderBook.OrderBook()
            {
                Asks = new[]
                {
                    new VolumePrice(){Price = 5, Volume = 1},
                    new VolumePrice(){Price = 4, Volume = 1},
                },
                Bids = new[]
                {
                    new VolumePrice(){Price = 2, Volume = 1},
                    new VolumePrice(){Price = 1, Volume = 1},
                },
            };
        }

        private ICompositeOrderBook GetCompositeOrderBook()
        {
            var mock = new Mock<ICompositeOrderBook>();
            mock.Setup(b => b.Asks).Returns(new[]
            {
                new SourcedVolumePrice(5, 1, Source),
                new SourcedVolumePrice(4, 1, Source)
            });
            mock.Setup(b => b.Bids).Returns(new[]
            {
                new SourcedVolumePrice(2, 1, Source),
                new SourcedVolumePrice(1, 1, Source)
            });
            return mock.Object;
        }

        private BestPriceEvaluator GetBestPriceEvaluator(Core.Domain.LykkeOrderBook.OrderBook orderBook, 
            decimal minHalfSpread = 0)
        {
            var lykkeExchangeMock = new Mock<ILykkeExchange>();
            lykkeExchangeMock.Setup(e => e.GetOrderBook(AssetPairId)).Returns(orderBook);

            var compositeExchange = new CompositeExchange();

            var assetPairDictionaryMock = new Mock<IAssetPairDictionary>();
            assetPairDictionaryMock.Setup(d => d[AssetPairId])
                .Returns(new AssetPair() { QuotingAssetId = QuotingAssetId });

            var assetMinHalfSpread = new Dictionary<string, decimal>()
            {
                {QuotingAssetId, minHalfSpread}
            };

            return new BestPriceEvaluator(lykkeExchangeMock.Object,
                compositeExchange,
                assetPairDictionaryMock.Object,
                assetMinHalfSpread);
        }

        [Fact]
        public void GetMinAsk_WhenLykkeExchangeContainsOrderBook_ReturnsLykkeMiddlePrice()
        {
            var orderBook = GetOrderBook();
            var bestPriceEvaluator = GetBestPriceEvaluator(orderBook);

            decimal result = bestPriceEvaluator.GetMinAsk(AssetPairId);

            var middle = (orderBook.Asks.Min(p => p.Price) + orderBook.Bids.Max(p => p.Price)) / 2;
            Assert.Equal(result, middle);
        }

        private BestPriceEvaluator GetBestPriceEvaluator(ICompositeOrderBook compositeOrderBook,
            decimal minHalfSpread = 0)
        {
            var lykkeExchangeMock = new Mock<ILykkeExchange>();

            var compositeExchange = new CompositeExchange() {
                { AssetPairId, compositeOrderBook }
            };

            var assetPairDictionaryMock = new Mock<IAssetPairDictionary>();
            assetPairDictionaryMock.Setup(d => d[AssetPairId])
                .Returns(new AssetPair() { QuotingAssetId = QuotingAssetId });

            var assetMinHalfSpread = new Dictionary<string, decimal>()
            {
                {QuotingAssetId, minHalfSpread}
            };

            return new BestPriceEvaluator(lykkeExchangeMock.Object,
                compositeExchange,
                assetPairDictionaryMock.Object,
                assetMinHalfSpread);
        }

        [Fact]
        public void GetMinAsk_WhenLykkeExchangeDoesNotContainOrderBook_ReturnsExternalMiddlePrice()
        {
            var compositeOrderBook = GetCompositeOrderBook();
            var bestPriceEvaluator = GetBestPriceEvaluator(compositeOrderBook);

            decimal result = bestPriceEvaluator.GetMinAsk(AssetPairId);

            var middle = (compositeOrderBook.Asks.Min(p => p.Price) + compositeOrderBook.Bids.Max(p => p.Price)) / 2;
            Assert.Equal(result, middle);
        }

        [Fact]
        public void GetMinAsk_WhenMinHalfSpreadIsSet_ReturnsLykkeMiddlePricePlusSpread()
        {
            var orderBook = GetOrderBook();
            var bestPriceEvaluator = GetBestPriceEvaluator(orderBook, MinHalfSpread);

            decimal result = bestPriceEvaluator.GetMinAsk(AssetPairId);

            var middle = (orderBook.Asks.Min(p => p.Price) + orderBook.Bids.Max(p => p.Price)) / 2 + MinHalfSpread;
            Assert.Equal(result, middle);
        }

        [Fact]
        public void GetMaxBid_WhenLykkeExchangeContainsOrderBook_ReturnsLykkeMiddlePrice()
        {
            var orderBook = GetOrderBook();
            var bestPriceEvaluator = GetBestPriceEvaluator(orderBook);

            decimal result = bestPriceEvaluator.GetMaxBid(AssetPairId);

            var middle = (orderBook.Asks.Min(p => p.Price) + orderBook.Bids.Max(p => p.Price)) / 2;
            Assert.Equal(result, middle);
        }

        [Fact]
        public void GetMaxBid_WhenLykkeExchangeDoesNotContainOrderBook_ReturnsExternalMiddlePrice()
        {
            var compositeOrderBook = GetCompositeOrderBook();
            var bestPriceEvaluator = GetBestPriceEvaluator(compositeOrderBook);

            decimal result = bestPriceEvaluator.GetMaxBid(AssetPairId);

            var middle = (compositeOrderBook.Asks.Min(p => p.Price) + compositeOrderBook.Bids.Max(p => p.Price)) / 2;
            Assert.Equal(result, middle);
        }

        [Fact]
        public void GetMaxBid_WhenMinHalfSpreadIsSet_ReturnsLykkeMiddlePriceMinusSpread()
        {
            var orderBook = GetOrderBook();
            var bestPriceEvaluator = GetBestPriceEvaluator(orderBook, MinHalfSpread);

            decimal result = bestPriceEvaluator.GetMaxBid(AssetPairId);

            var middle = (orderBook.Asks.Min(p => p.Price) + orderBook.Bids.Max(p => p.Price)) / 2 - MinHalfSpread;
            Assert.Equal(result, middle);
        }
    }
}
