using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests.Filters.VolumePrice
{
    public class BestPriceFilterTests
    {
        private const string Source = "bitfinex";
        private const string AssetPairId = "ETHBTC";
        private const decimal Price = 3;

        private IEnumerable<SourcedVolumePrice> GetPrices()
        {
            return new[]
            {
                new SourcedVolumePrice(Price, 1, Source),
            };
        }

        private IBestPriceEvaluator GetBestPriceEvaluator(decimal bestPrice)
        {
            var bestPriceEvaluator = new Mock<IBestPriceEvaluator>();
            bestPriceEvaluator.Setup(e => e.GetMinAsk(It.IsAny<string>())).Returns(bestPrice);
            bestPriceEvaluator.Setup(e => e.GetMaxBid(It.IsAny<string>())).Returns(bestPrice);
            return bestPriceEvaluator.Object;
        }

        [Fact]
        public void GetAsks_WhenPriceLessThenMinAsk_ReturnsMinAsk()
        {
            decimal minAsk = Price + 1;
            var bestPriceFilter = new BestPriceFilter(GetBestPriceEvaluator(minAsk));

            var result = bestPriceFilter.GetAsks(AssetPairId, GetPrices());

            Assert.Equal(result.First().Price, minAsk);
        }

        [Fact]
        public void GetAsks_WhenPriceMoreThenMinAsk_ReturnsPrice()
        {
            decimal minAsk = Price - 1;
            var bestPriceFilter = new BestPriceFilter(GetBestPriceEvaluator(minAsk));

            var result = bestPriceFilter.GetAsks(AssetPairId, GetPrices());

            Assert.Equal(result.First().Price, Price);
        }

        [Fact]
        public void GetBids_WhenPriceMoreThenMaxBid_ReturnsMaxBid()
        {
            decimal maxBid = Price - 1;
            var bestPriceFilter = new BestPriceFilter(GetBestPriceEvaluator(maxBid));

            var result = bestPriceFilter.GetBids(AssetPairId, GetPrices());

            Assert.Equal(result.First().Price, maxBid);
        }

        [Fact]
        public void GetBids_WhenPriceLessThenMaxBid_ReturnsPrice()
        {
            decimal maxBid = Price + 1;
            var bestPriceFilter = new BestPriceFilter(GetBestPriceEvaluator(maxBid));

            var result = bestPriceFilter.GetBids(AssetPairId, GetPrices());

            Assert.Equal(result.First().Price, Price);
        }
    }
}
