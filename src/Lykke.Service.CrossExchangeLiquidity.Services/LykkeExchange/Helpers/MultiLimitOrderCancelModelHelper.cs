using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange.Helpers
{
    public class MultiLimitOrderCancelModelHelper : MatchingEngineClientHelper
    {
        private readonly IClientIdSettings _settings;

        public MultiLimitOrderCancelModelHelper(IClientIdSettings settings)
        {
            _settings = settings;
        }

        public MultiLimitOrderCancelModel CreateMultiLimitOrderCancelModel(string assetPairId, bool isBuy)
        {
            return new MultiLimitOrderCancelModel()
            {
                Id = CreateRequestId(),
                ClientId = _settings.ClientId,
                AssetPairId = assetPairId,
                IsBuy = isBuy
            };
        }

        public string ToString(MultiLimitOrderCancelModel model)
        {
            if (model == null)
                return string.Empty;

            return $"{model.GetType().Name} " +
                   $"{nameof(model.Id)} = {model.Id} " +
                   $"{nameof(model.ClientId)} = {model.ClientId} " +
                   $"{nameof(model.AssetPairId)} = {model.AssetPairId} " +
                   $"{nameof(model.IsBuy)} = {model.IsBuy}";
        }
    }
}
