using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice
{
    public class ExternalBalanceVolumePriceFilter: IVolumePriceFilter
    {
        private readonly IExternalBalanceService _externalBalanceService;
        private readonly IAssetPairDictionary _assetPairDictionary;

        public ExternalBalanceVolumePriceFilter(IExternalBalanceService externalBalanceService,
            IAssetPairDictionary assetPairDictionary)
        {
            _externalBalanceService = externalBalanceService;
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
            var balances = new Dictionary<string, decimal>();
            foreach (var volumePrice in prices)
            {
                decimal balance;
                if (balances.ContainsKey(volumePrice.Source))
                {
                    balance = balances[volumePrice.Source];
                }
                else
                {
                    balance = _externalBalanceService.GetAssetBalance(volumePrice.Source, assetId);
                }

                balance -= volumePrice.Volume;
                if (balance >= 0)
                {
                    yield return volumePrice;
                }

                balances[volumePrice.Source] = balance;
            }
        }
    }
}
