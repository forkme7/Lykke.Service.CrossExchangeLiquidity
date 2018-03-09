﻿using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange.Helpers
{
    public class MultiLimitOrderModelHelper : MatchingEngineClientHelper
    {
        private readonly IClientIdSettings _settings;
        private readonly ITradeFilter _filter;

        public MultiLimitOrderModelHelper(IClientIdSettings settings, ITradeFilter filter)
        {
            _settings = settings;
            _filter = filter;
        }

        public MultiLimitOrderModel CreateMultiLimitOrderModel(IOrderBook orderBook)
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
                orders.Add(CreateMultiOrderItemModel(OrderAction.Sell, volumePrice));
            }

            foreach (VolumePrice volumePrice in _filter.GetBids(orderBook))
            {
                orders.Add(CreateMultiOrderItemModel(OrderAction.Buy, volumePrice));
            }

            model.Orders = orders;

            return model;
        }

        public MultiOrderItemModel CreateMultiOrderItemModel(OrderAction orderAction, VolumePrice volumePrice)
        {
            return new MultiOrderItemModel()
            {
                Id = CreateRequestId(),
                OrderAction = orderAction,
                Volume = (double)volumePrice.Volume,
                Price = (double)volumePrice.Price,
                Fee = null
            };
        }

        public string ToString(MultiLimitOrderModel model)
        {
            if (model == null)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine($"{model.GetType().Name}");
            sb.AppendLine($"{nameof(model.Id)} = {model.Id}");
            sb.AppendLine($"{nameof(model.ClientId)} = {model.ClientId}");
            sb.AppendLine($"{nameof(model.AssetId)} = {model.AssetId}");
            sb.AppendLine($"{nameof(model.CancelPreviousOrders)} = {model.CancelPreviousOrders}");

            foreach (MultiOrderItemModel order in model.Orders)
            {
                sb.AppendLine(ToString(order));
            }

            return sb.ToString();
        }

        public string ToString(MultiOrderItemModel model)
        {
            if (model == null)
                return string.Empty;

            return $"{model.GetType().Name} " +
                   $"{nameof(model.Id)} = {model.Id} " +
                   $"{nameof(model.OrderAction)} = {model.OrderAction} " +
                   $"{nameof(model.Price)} = {model.Price} " +
                   $"{nameof(model.Volume)} = {model.Volume} " +
                   ToString(model.Fee);
        }

        public string ToString(LimitOrderFeeModel model)
        {
            if (model == null)
                return string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine($"{model.GetType().Name}");
            sb.AppendLine($"{nameof(model.Type)} = {model.Type}");
            sb.AppendLine($"{nameof(model.SourceClientId)} = {model.SourceClientId}");
            sb.AppendLine($"{nameof(model.TargetClientId)} = {model.TargetClientId}");
            sb.AppendLine($"{nameof(model.MakerSize)} = {model.MakerSize}");
            sb.AppendLine($"{nameof(model.MakerSizeType)} = {model.MakerSizeType}");
            sb.AppendLine($"{nameof(model.TakerSize)} = {model.TakerSize}");
            sb.AppendLine($"{nameof(model.TakerSizeType)} = {model.TakerSizeType}");
            sb.AppendLine($"{nameof(model.MakerFeeModificator)} = {model.MakerFeeModificator}");

            foreach (string assetId in model.AssetId)
            {
                sb.AppendLine($"{nameof(model.AssetId)} = {assetId}");
            }

            return sb.ToString();
        }

    }
}