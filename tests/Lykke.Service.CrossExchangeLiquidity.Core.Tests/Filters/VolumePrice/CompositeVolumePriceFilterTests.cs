using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Moq;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests.Filters.VolumePrice
{
    public class CompositeVolumePriceFilterTests
    {
        private const string AssetPairId = "ETHBTC";

        private IEnumerable<SourcedVolumePrice>[] GetPrices()
        {
            return new IEnumerable<SourcedVolumePrice>[]
            {
                new[] {new SourcedVolumePrice(1, 1, AssetPairId)},
                new[] {new SourcedVolumePrice(2, 2, AssetPairId)}
            };
        }

        private CompositeVolumePriceFilter GetCompositeVolumePriceFilter(params IEnumerable<SourcedVolumePrice>[] results)
        {
            var filterList = new List<IVolumePriceFilter>();
            foreach (IEnumerable<SourcedVolumePrice> result in results)
            {
                var filter = new Mock<IVolumePriceFilter>();
                filter.Setup(f => f.GetAsks(It.IsAny<string>(), It.IsAny<IEnumerable<SourcedVolumePrice>>()))
                    .Returns<string, IEnumerable<SourcedVolumePrice>>((assetPairId, asks) => result.Union(asks));
                filter.Setup(f => f.GetBids(It.IsAny<string>(), It.IsAny<IEnumerable<SourcedVolumePrice>>()))
                    .Returns<string, IEnumerable<SourcedVolumePrice>>((assetPairId, bids) => result.Union(bids));

                filterList.Add(filter.Object);
            }

            return new CompositeVolumePriceFilter(filterList);
        }

        [Fact]
        public void GetAsks_WhenFiltersAreAdded_RetrunsAll()
        {
            var prices = GetPrices();
            var compositeVolumePriceFilter = GetCompositeVolumePriceFilter(prices);

            IEnumerable<SourcedVolumePrice> result = compositeVolumePriceFilter.GetAsks(AssetPairId, 
                new SourcedVolumePrice[0]);

            Assert.Equal(prices.SelectMany(p=>p).Count(), result.Count());
        }

        [Fact]
        public void GetBids_WhenFiltersAreAdded_RetrunsAll()
        {
            var prices = GetPrices();

            var compositeVolumePriceFilter = GetCompositeVolumePriceFilter(prices);

            IEnumerable<SourcedVolumePrice> result = compositeVolumePriceFilter.GetBids(AssetPairId,
                new SourcedVolumePrice[0]);

            Assert.Equal(prices.SelectMany(p => p).Count(), result.Count());
        }
    }
}
