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
    public class TopVolumePriceFilterTests
    {
        private const int Count = 2;        
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

        [Fact]
        public void GetAsks_WhenFiltered_ReturnsTopPrices()
        {
            var topVolumePriceFilter = new TopVolumePriceFilter(Count);

            var result = topVolumePriceFilter.GetAsks(AssetPairId, GetPrices());

            Assert.Equal(result.Count(), Count);
        }

        [Fact]
        public void GetBids_WhenFiltered_ReturnsTopPrices()
        {
            var topVolumePriceFilter = new TopVolumePriceFilter(Count);

            var result = topVolumePriceFilter.GetBids(AssetPairId, GetPrices());

            Assert.Equal(result.Count(), Count);
        }
    }
}
