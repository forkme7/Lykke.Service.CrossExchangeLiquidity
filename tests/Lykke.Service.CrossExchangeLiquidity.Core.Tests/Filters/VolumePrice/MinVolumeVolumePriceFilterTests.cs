﻿using System;
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
    public class MinVolumeVolumePriceFilterTests
    {
        private const decimal Min = 3;
        private const string BaseAssetId = "BTC";
        private const string AssetPairId = "ETHBTC";
        private const string Source = "bitfinex";

        private IDictionary<string, decimal> GetAssetMinVolumes()
        {
            return new Dictionary<string, decimal>()
            {
                { BaseAssetId, Min }
            };
        }

        private IAssetPairDictionary GetAssetPairDictionary()
        {
            var assetPairDictionaryMock = new Mock<IAssetPairDictionary>();
            assetPairDictionaryMock.Setup(p => p[It.IsAny<string>()])
                .Returns(new AssetPair()
                {
                    BaseAssetId = BaseAssetId
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
        public void GetAsks_WhenVolumeIsLessThenMin_SkipThisOrder()
        {
            var minVolumeVolumePriceFilter = new MinVolumeVolumePriceFilter(GetAssetMinVolumes(), 
                GetAssetPairDictionary());

            var result = minVolumeVolumePriceFilter.GetAsks(AssetPairId, GetPrices());

            Assert.All(result, p=>  Assert.True(p.Volume >= Min));
        }

        [Fact]
        public void GetBids_WhenVolumeIsLessThenMin_SkipThisOrder()
        {
            var minVolumeVolumePriceFilter = new MinVolumeVolumePriceFilter(GetAssetMinVolumes(),
                GetAssetPairDictionary());

            var result = minVolumeVolumePriceFilter.GetBids(AssetPairId, GetPrices());

            Assert.All(result, p => Assert.True(p.Volume >= Min));
        }
    }
}
