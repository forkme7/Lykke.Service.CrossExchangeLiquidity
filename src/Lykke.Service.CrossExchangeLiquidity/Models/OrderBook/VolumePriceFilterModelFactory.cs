using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook.Filters;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lykke.Service.CrossExchangeLiquidity.Models.OrderBook
{
    public class VolumePriceFilterModelFactory : IVolumePriceFilterModelFactory
    {
        public VolumePriceFilterModel GetModel(IVolumePriceFilter filter, string assetPairId)
        {
            switch (filter)
            {
                case VolumePartVolumePriceFilter f: return GetModel(f, assetPairId);
                case TopVolumePriceFilter f: return GetModel(f, assetPairId);
                case RiskMarkupVolumePriceFilter f: return GetModel(f, assetPairId);
                case MinVolumeVolumePriceFilter f: return GetModel(f, assetPairId);
                case LykkeBalanceVolumePriceFilter f: return GetModel(f, assetPairId);
                case ExternalBalanceVolumePriceFilter f: return GetModel(f, assetPairId);
                case BestPriceFilter f: return GetModel(f, assetPairId);
                default: var model = new VolumePriceFilterModel();
                    SetTitle(model, filter);
                    return model;
            }
        }

        private void SetTitle(VolumePriceFilterModel model, IVolumePriceFilter filter)
        {
            model.Title = filter.GetType().Name;
        }

        public VolumePartVolumePriceFilterModel GetModel(VolumePartVolumePriceFilter filter, string assetPairId)
        {
            var model = new VolumePartVolumePriceFilterModel();
            SetTitle(model, filter);
            model.VolumePart = filter.GetVolumePart(assetPairId);
            return model;
        }

        public TopVolumePriceFilterModel GetModel(TopVolumePriceFilter filter, string assetPairId)
        {
            var model = new TopVolumePriceFilterModel();
            SetTitle(model, filter);
            model.Count = filter.Count;
            return model;
        }

        public RiskMarkupVolumePriceFilterModel GetModel(RiskMarkupVolumePriceFilter filter, string assetPairId)
        {
            var model = new RiskMarkupVolumePriceFilterModel();
            SetTitle(model, filter);
            model.RiskMarkup = filter.GetRiskMarkup(assetPairId);
            return model;
        }

        public MinVolumeVolumePriceFilterModel GetModel(MinVolumeVolumePriceFilter filter, string assetPairId)
        {
            var model = new MinVolumeVolumePriceFilterModel();
            SetTitle(model, filter);
            model.MinVolume = filter.GetMinVolume(assetPairId);
            return model;
        }

        public LykkeBalanceVolumePriceFilterModel GetModel(LykkeBalanceVolumePriceFilter filter, string assetPairId)
        {
            var model = new LykkeBalanceVolumePriceFilterModel();
            SetTitle(model, filter);
            AssetPair assetPair = filter.AssetPairDictionary[assetPairId];
            model.LykkeBalance = new ReadOnlyDictionary<string, decimal>(new Dictionary<string, decimal>
            {
                {assetPair.BaseAssetId, filter.LykkeBalanceService.GetAssetBalance(assetPair.BaseAssetId)},
                {assetPair.QuotingAssetId, filter.LykkeBalanceService.GetAssetBalance(assetPair.QuotingAssetId)},
            });
            return model;
        }

        public ExternalBalanceVolumePriceFilterModel GetModel(ExternalBalanceVolumePriceFilter filter, string assetPairId)
        {
            var model = new ExternalBalanceVolumePriceFilterModel();
            SetTitle(model, filter);
            AssetPair assetPair = filter.AssetPairDictionary[assetPairId];

            ReadOnlyDictionary<string, ReadOnlyDictionary<string,decimal>> balances = filter.ExternalBalanceService.GetBalances();
            var baseAssetBalance = new Dictionary<string, decimal>();
            var quotingAssetBalance = new Dictionary<string, decimal>();
            foreach (var source in balances.Keys)
            {
                ReadOnlyDictionary<string, decimal> sourceBalances = balances[source];
                if (sourceBalances.TryGetValue(assetPair.BaseAssetId, out var baseBalance))
                {
                    baseAssetBalance.Add(source, baseBalance);
                }
                if (sourceBalances.TryGetValue(assetPair.QuotingAssetId, out var quotingBalance))
                {
                    baseAssetBalance.Add(source, quotingBalance);
                }
            }

            model.ExternalBalance = new ReadOnlyDictionary<string, ReadOnlyDictionary<string, decimal>>(new Dictionary<string, ReadOnlyDictionary<string, decimal>>
            {
                {assetPair.BaseAssetId, new ReadOnlyDictionary<string, decimal>(baseAssetBalance)},
                {assetPair.QuotingAssetId, new ReadOnlyDictionary<string, decimal>(quotingAssetBalance)}
            });
            return model;
        }

        public BestPriceFilterModel GetModel(BestPriceFilter filter, string assetPairId)
        {
            var model = new BestPriceFilterModel();
            SetTitle(model, filter);
            model.MinAsk = filter.BestPriceEvaluator.GetMinAsk(assetPairId);
            model.MaxBid = filter.BestPriceEvaluator.GetMinAsk(assetPairId);
            return model;
        }
    }
}
