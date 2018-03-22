using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Moq;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests.Filters.VolumePrice
{
    public class LykkeBalanceVolumePriceFilterTests
    {
        private const decimal Balance = 10;
        private const string QuotingAssetId = "ETH";
        private const string BaseAssetId = "BTC";
        private const string AssetPairId = "ETHBTC";
        private const string Source = "bitfinex";

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

        private ILykkeBalanceService GetLykkeBalanceService(decimal baseBalance,
            decimal quotingBalance)
        {
            var lykkeBalanceServiceMock = new Mock<ILykkeBalanceService>();
            lykkeBalanceServiceMock.Setup(s => s.GetAssetBalance(BaseAssetId))
                .Returns(baseBalance);
            lykkeBalanceServiceMock.Setup(s => s.GetAssetBalance(QuotingAssetId))
                .Returns(quotingBalance);
            return lykkeBalanceServiceMock.Object;
        }

        private IAssetPairDictionary GetAssetPairDictionary()
        {
            var assetPairDictionaryMock = new Mock<IAssetPairDictionary>();
            assetPairDictionaryMock.Setup(p => p[It.IsAny<string>()])
                .Returns(new AssetPair()
                {
                    QuotingAssetId = QuotingAssetId,
                    BaseAssetId = BaseAssetId
                });
            return assetPairDictionaryMock.Object;
        }

        [Fact]
        public void GetAsks_WhenFiltered_RetrunsOnlyAfforded()
        {
            var lykkeBalanceVolumePriceFilter = new LykkeBalanceVolumePriceFilter(
                GetLykkeBalanceService(Balance, 0),
                GetAssetPairDictionary());

            var result = lykkeBalanceVolumePriceFilter.GetAsks(AssetPairId, GetPrices());

            Assert.InRange(result.Sum(p => p.Price), 1, Balance);
        }

        [Fact]
        public void GetBids_WhenFiltered_RetrunsOnlyAfforded()
        {
            var lykkeBalanceVolumePriceFilter = new LykkeBalanceVolumePriceFilter(
                GetLykkeBalanceService(0, Balance),
                GetAssetPairDictionary());

            var result = lykkeBalanceVolumePriceFilter.GetBids(AssetPairId, GetPrices());

            Assert.InRange(result.Sum(p => p.Price * p.Volume), 1, Balance);
        }
    }
}
