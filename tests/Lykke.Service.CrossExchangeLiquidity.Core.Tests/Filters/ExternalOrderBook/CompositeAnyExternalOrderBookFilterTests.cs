using System;
using System.Collections.Generic;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook;
using Moq;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests.Filters.ExternalOrderBook
{
    public class CompositeAnyExternalOrderBookFilterTests
    {
        private const string Source = "bitfinex";
        private const string AssetPairId1 = "ETHBTC";

        private Core.Domain.ExternalOrderBook.ExternalOrderBook GetOrderBook()
        {
            return new Core.Domain.ExternalOrderBook.ExternalOrderBook(
                Source,
                AssetPairId1,
                new ExternalVolumePrice[0],
                new ExternalVolumePrice[0],
                DateTime.Now);
        }

        private CompositeAnyExternalOrderBookFilter GetCompositeAnyExternalOrderBookFilter(params bool[] acceptedList)
        {
            var filterList = new List<IExternalOrderBookFilter>();
            foreach (bool accepted in acceptedList)
            {
                var filter = new Mock<IExternalOrderBookFilter>();
                filter.Setup(f => f.IsAccepted(It.IsAny<IExternalOrderBook>())).Returns(accepted);

                filterList.Add(filter.Object);
            }

            return new CompositeAnyExternalOrderBookFilter(filterList);
        }

        [Fact]
        public void IsAccepted_WhenAnyFilterIsAccepted_RetrunsTrue()
        {
            var compositeAnyExternalOrderBookFilter = GetCompositeAnyExternalOrderBookFilter(false, true);

            bool result =
                compositeAnyExternalOrderBookFilter.IsAccepted(GetOrderBook());

            Assert.True(result);
        }

        [Fact]
        public void IsAccepted_WhenNoFiltersIsAccepted_RetrunsFalse()
        {
            var compositeAnyExternalOrderBookFilter = GetCompositeAnyExternalOrderBookFilter(false, false);

            bool result =
                compositeAnyExternalOrderBookFilter.IsAccepted(GetOrderBook());

            Assert.False(result);
        }
    }
}
