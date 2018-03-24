using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class LykkeBalanceVolumePriceFilter : IVolumePriceFilter
    {
        public ILykkeBalanceService LykkeBalanceService { get; }
        public IAssetPairDictionary AssetPairDictionary;

        public LykkeBalanceVolumePriceFilter(ILykkeBalanceService lykkeBalanceService,
            IAssetPairDictionary assetPairDictionary)
        {
            LykkeBalanceService = lykkeBalanceService;
            AssetPairDictionary = assetPairDictionary;
        }

        public IEnumerable<SourcedVolumePrice> GetAsks(string assetPairId, IEnumerable<SourcedVolumePrice> asks)
        {
            return GetPrices(assetPairId, false, asks);
        }

        public IEnumerable<SourcedVolumePrice> GetBids(string assetPairId, IEnumerable<SourcedVolumePrice> bids)
        {
            return GetPrices(assetPairId, true, bids);
        }

        private IEnumerable<SourcedVolumePrice> GetPrices(string assetPairId, 
            bool isBuy,
            IEnumerable<SourcedVolumePrice> prices)
        {
            AssetPair assetPair = AssetPairDictionary[assetPairId];
            string assetId = isBuy ? assetPair.QuotingAssetId : assetPair.BaseAssetId;

            decimal balance = LykkeBalanceService.GetAssetBalance(assetId);
            foreach (var volumePrice in prices)
            {
                if (isBuy)
                {
                    balance -= volumePrice.Volume * volumePrice.Price;
                }
                else
                {
                    balance -= volumePrice.Volume;
                }

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
