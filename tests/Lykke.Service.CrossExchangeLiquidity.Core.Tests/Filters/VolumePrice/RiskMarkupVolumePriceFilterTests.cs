using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Moq;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests.Filters.VolumePrice
{
    public class RiskMarkupVolumePriceFilterTests
    {
        private const decimal RiskMarkup = 1;
        private const string QuotingAssetId = "BTC";
        private const string AssetPairId = "ETHBTC";
        private const string Source = "bitfinex";

        private IDictionary<string, decimal> GetAssetRiskMarkup()
        {
            var assetMinVolumes = new Dictionary<string, decimal>
            {
                { QuotingAssetId, RiskMarkup }
            };
            return assetMinVolumes;
        }

        private IAssetPairDictionary GetAssetPairDictionary()
        {
            var assetPairDictionaryMock = new Mock<IAssetPairDictionary>();
            assetPairDictionaryMock.Setup(p => p[It.IsAny<string>()])
                .Returns(new AssetPair()
                {
                    QuotingAssetId = QuotingAssetId
                });
            return assetPairDictionaryMock.Object;
        }

        private IEnumerable<SourcedVolumePrice> GetPrices()
        {
            return new[]
            {
                new SourcedVolumePrice(1, 1, Source),
                new SourcedVolumePrice(2, 2, Source),
                new SourcedVolumePrice(3, 3, Source),
                new SourcedVolumePrice(4, 4, Source),
                new SourcedVolumePrice(5, 5, Source)
            };
        }

        [Fact]
        public void GetAsks_WhenFiltered_ReturnsIncreatedPrice()
        {
            var riskMarkupVolumePriceFilter = new RiskMarkupVolumePriceFilter(GetAssetRiskMarkup(),
                GetAssetPairDictionary());

            var result = riskMarkupVolumePriceFilter.GetAsks(AssetPairId, GetPrices());

            Assert.All(result, p => Assert.True(p.Price == p.Volume + RiskMarkup));
        }

        [Fact]
        public void GetBids_WhenFiltered_ReturnsReducedPrice()
        {
            var riskMarkupVolumePriceFilter = new RiskMarkupVolumePriceFilter(GetAssetRiskMarkup(),
                GetAssetPairDictionary());

            var result = riskMarkupVolumePriceFilter.GetBids(AssetPairId, GetPrices());

            Assert.All(result, p => Assert.True(p.Price == p.Volume - RiskMarkup));
        }
    }
}
