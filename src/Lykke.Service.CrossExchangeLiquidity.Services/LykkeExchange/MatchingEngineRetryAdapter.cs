using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange
{
    public class MatchingEngineRetryAdapter : IMatchingEngineClient
    {
        private readonly ILog _log;
        private readonly IMatchingEngineClient _matchingEngineClient;
        private readonly IRetrySettings _settings;

        public MatchingEngineRetryAdapter(IMatchingEngineClient matchingEngineClient,
            IRetrySettings settings,
            ILog log)
        {
            _matchingEngineClient = matchingEngineClient;
            _settings = settings;
            _log = log;
        }

        public bool IsConnected => _matchingEngineClient.IsConnected;

        public async Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId)
        {
            return await Execute(client => client.CancelLimitOrderAsync(limitOrderId));
        }

        public async Task<MeResponseModel> CancelMultiLimitOrderAsync(MultiLimitOrderCancelModel cancelModel)
        {
            return await Execute(client => client.CancelMultiLimitOrderAsync(cancelModel));
        }

        public async Task<MeResponseModel> CashInOutAsync(string id, string clientId, string assetId, double amount)
        {
            return await Execute(client => client.CashInOutAsync(id, clientId, assetId, amount));
        }

        public async Task<MeResponseModel> CashInOutAsync(string id, string clientId, string assetId, int accuracy, double amount, string feeClientId, double feeSize, FeeSizeType feeSizeType)
        {
            return await Execute(client => client.CashInOutAsync(id, clientId, assetId, accuracy, amount, feeClientId, feeSize, feeSizeType));
        }

        public async Task<string> HandleMarketOrderAsync(string clientId, string assetPairId, OrderAction orderAction, double volume, bool straight, double? reservedLimitVolume = null)
        {
            return await Execute(client => client.HandleMarketOrderAsync(clientId, assetPairId, orderAction, volume, straight, reservedLimitVolume));
        }

        public async Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model)
        {
            return await Execute(client => client.HandleMarketOrderAsync(model));
        }

        public async Task<MeResponseModel> PlaceLimitOrderAsync(LimitOrderModel order)
        {
            return await Execute(client => client.PlaceLimitOrderAsync(order));
        }

        public async Task<MultiLimitOrderResponse> PlaceMultiLimitOrderAsync(MultiLimitOrderModel order)
        {
            return await Execute(client => client.PlaceMultiLimitOrderAsync(order));
        }

        public async Task<MeResponseModel> SwapAsync(string id, string clientId1, string assetId1, double amount1, string clientId2, string assetId2, double amount2)
        {
            return await Execute(client => client.SwapAsync(id, clientId1, assetId1, amount1, clientId2, assetId2, amount2));
        }

        public async Task<MeResponseModel> TransferAsync(string id, string fromClientId, string toClientId, string assetId, int accuracy, double amount, string feeClientId, double feeSizePercentage, double overdraft)
        {
            return await Execute(client => client.TransferAsync(id, fromClientId, toClientId, assetId, accuracy, amount, feeClientId, feeSizePercentage, overdraft));
        }

        public async Task UpdateBalanceAsync(string id, string clientId, string assetId, double value)
        {
            await Execute(async (client) =>
            {
                await client.UpdateBalanceAsync(id, clientId, assetId, value);
                return (object)null;
            });
        }

        private async Task<T> Execute<T>(Func<IMatchingEngineClient, Task<T>> action)
        {
            TimeSpan retryPeriod = _settings.TimeSpan;
            for (var i = 0; i < _settings.Times; i++)
            {
                if (_matchingEngineClient.IsConnected)
                {
                    return await action(_matchingEngineClient);
                }
                await _log.WriteWarningAsync(nameof(MatchingEngineRetryAdapter), 
                                            nameof(Execute),
                                            $"Matching Engine client is not connected. Retry after {retryPeriod}...");
                await Task.Delay(retryPeriod);
                retryPeriod *= _settings.Multiplier;
            }

            throw new TimeoutException($"Connection to Matching Engine failed after {_settings.Times} retries.");
        }
    }
}
