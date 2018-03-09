using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange
{
    public class MatchingEngineTrader : ITrader
    {
        private readonly ILog _log;
        private readonly ITradeFilter _filter;
        private readonly IMatchingEngineClient _matchingEngineClient;
        private readonly ILykkeExchangeSettings _settings;
        private DateTime _lastPlace = DateTime.MinValue;
        private MultiLimitOrderModel _lastModel = null;
        private readonly MultiLimitOrderModelEqualityComparer _multiLimitOrderModelEqualityComparer;

        public MatchingEngineTrader(ILog log,
            ILykkeExchangeSettings settings, 
            IMatchingEngineClient matchingEngineClient,
            ITradeFilter filter)
        {
            _log = log;
            _settings = settings;
            _matchingEngineClient = matchingEngineClient;
            _filter = filter;
            _multiLimitOrderModelEqualityComparer = new MultiLimitOrderModelEqualityComparer();
        }

        public async Task PlaceOrders(IOrderBook orderBook)
        {
            MultiLimitOrderModel model = CreateMultiLimitOrderModel(orderBook);
            if(!IsModelChanged(model))
                return;

            if (!IsBreakOver())
                return;

            //todo: add logging
            if (model.Orders.All(o=>o.OrderAction == OrderAction.Buy))
            {
                await CancelMultiLimitOrderAsync(orderBook.AssetPairId, false);
            }
            else if (model.Orders.All(o => o.OrderAction == OrderAction.Sell))
            {
                await CancelMultiLimitOrderAsync(orderBook.AssetPairId, true);
            }

            await _matchingEngineClient.PlaceMultiLimitOrderAsync(model);
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

        private MultiLimitOrderModel CreateMultiLimitOrderModel(IOrderBook orderBook)
        {
            var model = new MultiLimitOrderModel()
            {
                Id = CreateRequestId(),
                ClientId = _settings.ClientId,
                AssetId = orderBook.AssetPairId,
                CancelPreviousOrders = true
            };

            var orders = new List<MultiOrderItemModel>();
            foreach (VolumePrice volumePrice in _filter.GetAsks(orderBook))
            {
                orders.Add(CreateMultiOrderItemModel(OrderAction.Buy, volumePrice));
            }

            foreach (VolumePrice volumePrice in _filter.GetBids(orderBook))
            {
                orders.Add(CreateMultiOrderItemModel(OrderAction.Sell, volumePrice));
            }

            model.Orders = orders;

            return model;
        }

        private string CreateRequestId()
        {
            return Guid.NewGuid().ToString();
        }

        private MultiOrderItemModel CreateMultiOrderItemModel(OrderAction orderAction, VolumePrice volumePrice)
        {
            return new MultiOrderItemModel()
            {
                Id = CreateRequestId(),
                OrderAction = orderAction,
                Volume = (double) volumePrice.Volume,
                Price = (double) volumePrice.Price,
                Fee = null
            };
        }

        private async Task CancelMultiLimitOrderAsync(string assetPairId, bool isBuy)
        {
            var model = new MultiLimitOrderCancelModel()
            {
                Id = CreateRequestId(),
                ClientId = _settings.ClientId,
                AssetPairId = assetPairId,
                IsBuy = isBuy
            };

            await _matchingEngineClient.CancelMultiLimitOrderAsync(model);
        }
    }
}
