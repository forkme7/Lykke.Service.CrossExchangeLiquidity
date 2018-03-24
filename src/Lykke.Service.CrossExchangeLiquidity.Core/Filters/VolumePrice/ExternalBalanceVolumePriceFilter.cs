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
        public IExternalBalanceService ExternalBalanceService { get; }
        public IAssetPairDictionary AssetPairDictionary { get; }

        public ExternalBalanceVolumePriceFilter(IExternalBalanceService externalBalanceService,
            IAssetPairDictionary assetPairDictionary)
        {
            ExternalBalanceService = externalBalanceService;
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
                    balance = ExternalBalanceService.GetAssetBalance(volumePrice.Source, assetId);
                }

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

                balances[volumePrice.Source] = balance;
            }
        }
    }
}
