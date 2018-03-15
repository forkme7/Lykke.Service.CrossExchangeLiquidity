using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class LykkeBalanceVolumePriceFilter : IVolumePriceFilter
    {
        private readonly ILykkeBalanceService _lykkeBalanceService;
        private readonly IAssetPairDictionary _assetPairDictionary;

        public LykkeBalanceVolumePriceFilter(ILykkeBalanceService lykkeBalanceService,
            IAssetPairDictionary assetPairDictionary)
        {
            _lykkeBalanceService = lykkeBalanceService;
            _assetPairDictionary = assetPairDictionary;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            AssetPair assetPair = _assetPairDictionary[assetPairId];
            return GetPrices(assetPair.BaseAssetId, asks);
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            AssetPair assetPair = _assetPairDictionary[assetPairId];
            return GetPrices(assetPair.QuotingAssetId, bids);
        }

        private IEnumerable<SourcedVolumePrice> GetPrices(string assetId, IEnumerable<SourcedVolumePrice> prices)
        {
            decimal balance = _lykkeBalanceService.GetAssetBalance(assetId);
            foreach (var volumePrice in prices)
            {
                balance -= volumePrice.Volume;
                if (balance >= 0)
                {
                    yield return volumePrice;
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
