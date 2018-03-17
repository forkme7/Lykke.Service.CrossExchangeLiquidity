using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange.Helpers;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange
{
    public class MatchingEngineTrader : ILykkeTrader
    {
        private readonly ILog _log;
        private readonly IMatchingEngineClient _matchingEngineClient;
        private readonly IMatchingEngineTraderSettings _settings;
        private DateTime _lastPlace = DateTime.MinValue;
        private MultiLimitOrderModel _lastModel = null;
        private readonly MultiLimitOrderModelHelper _multiLimitOrderModelHelper;
        private readonly MultiLimitOrderCancelModelHelper _multiLimitOrderCancelModelHelper;
        private readonly MultiLimitOrderModelEqualityComparer _multiLimitOrderModelEqualityComparer;

        public MatchingEngineTrader(ILog log,
            IMatchingEngineTraderSettings settings,
            IMatchingEngineClient matchingEngineClient,
            IBestPriceEvaluator bestPriceEvaluator,
            IVolumePriceFilter filter)
        {
            _log = log;
            _settings = settings;
            _matchingEngineClient = matchingEngineClient;
            _multiLimitOrderModelHelper = new MultiLimitOrderModelHelper(settings, filter, bestPriceEvaluator);
            _multiLimitOrderCancelModelHelper = new MultiLimitOrderCancelModelHelper(settings);
            _multiLimitOrderModelEqualityComparer = new MultiLimitOrderModelEqualityComparer();
        }

        public async Task PlaceOrdersAsync(ICompositeOrderBook orderBook)
        {
            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, $">> {orderBook}");

            MultiLimitOrderModel model = _multiLimitOrderModelHelper.CreateMultiLimitOrderModel(orderBook);

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name,
                $">> {_multiLimitOrderModelHelper.ToString(model)}");

            if (!IsModelChanged(model))
            {
                await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name,
                    "Model is not changed. Skip it.");
                return;
            }

            if (!IsBreakOver())
            {
                await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name,
                    $"Break is not over. The last place is {_lastPlace.ToShortTimeString()}.");
                return;
            }

            if (model.Orders.All(o => o.OrderAction == OrderAction.Buy))
            {
                await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name,
                    "Model contains only buy orders. Cancel previous sell orders.");

                await CancelMultiLimitOrderAsync(orderBook.AssetPairId, false);
            }
            else if (model.Orders.All(o => o.OrderAction == OrderAction.Sell))
            {
                await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name,
                    "Model contains only sell orders. Cancel previous buy orders.");

                await CancelMultiLimitOrderAsync(orderBook.AssetPairId, true);
            }

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name,
                "Place orders on the market.");

            await _matchingEngineClient.PlaceMultiLimitOrderAsync(model);

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, "<<");
        }

        private bool IsBreakOver()
        {
            var result = DateTime.Now - _lastPlace > _settings.TimeSpan;
            if (result)
            {
                _lastPlace = DateTime.Now;
            }

            return result;
        }

        private bool IsModelChanged(MultiLimitOrderModel model)
        {
            if (_multiLimitOrderModelEqualityComparer.Equals(model, _lastModel))
                return false;

            _lastModel = model;
            return true;
        }

        private async Task CancelMultiLimitOrderAsync(string assetPairId, bool isBuy)
        {
            MultiLimitOrderCancelModel model =
                _multiLimitOrderCancelModelHelper.CreateMultiLimitOrderCancelModel(assetPairId, isBuy);

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name,
                $">> {_multiLimitOrderCancelModelHelper.ToString(model)}");

            await _matchingEngineClient.CancelMultiLimitOrderAsync(model);

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, "<<");
        }
    }
}
