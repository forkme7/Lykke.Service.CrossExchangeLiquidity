using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange
{
    public class MatchingEngineRetryAdapter : IMatchingEngineClient
    {
        private readonly ILog _log;
        private readonly IMatchingEngineClient _matchingEngineClient;
        private readonly IMatchingEngineRetryAdapterSettings _settings;

        public MatchingEngineRetryAdapter(IMatchingEngineClient matchingEngineClient,
            IMatchingEngineRetryAdapterSettings settings,
            ILog log)
        {
            _matchingEngineClient = matchingEngineClient;
            _settings = settings;
            _log = log;
        }

        public bool IsConnected => _matchingEngineClient.IsConnected;

        public async Task<MeResponseModel> CancelLimitOrderAsync(string limitOrderId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.CancelLimitOrderAsync(limitOrderId, cancellationToken));
        }

        public async Task<MeResponseModel> CancelMultiLimitOrderAsync(MultiLimitOrderCancelModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.CancelMultiLimitOrderAsync(model, cancellationToken));
        }

        public async Task<MeResponseModel> CashInOutAsync(string id, string clientId, string assetId, double amount, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.CashInOutAsync(id, clientId, assetId, amount, cancellationToken));
        }

        public async Task<MeResponseModel> CashInOutAsync(string id, string clientId, string assetId, int accuracy, double amount, string feeClientId, double feeSize, FeeSizeType feeSizeType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.CashInOutAsync(id, clientId, assetId, accuracy, amount, feeClientId, feeSize, feeSizeType, cancellationToken));
        }

        public async Task<string> HandleMarketOrderAsync(string clientId, string assetPairId, OrderAction orderAction, double volume, bool straight, double? reservedLimitVolume = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.HandleMarketOrderAsync(clientId, assetPairId, orderAction, volume, straight, reservedLimitVolume, cancellationToken));
        }

        public async Task<MarketOrderResponse> HandleMarketOrderAsync(MarketOrderModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.HandleMarketOrderAsync(model, cancellationToken));
        }

        public async Task<MeResponseModel> PlaceLimitOrderAsync(LimitOrderModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.PlaceLimitOrderAsync(model, cancellationToken));
        }

        public async Task<MultiLimitOrderResponse> PlaceMultiLimitOrderAsync(MultiLimitOrderModel model, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.PlaceMultiLimitOrderAsync(model, cancellationToken));
        }

        public async Task<MeResponseModel> SwapAsync(string id, string clientId1, string assetId1, double amount1, string clientId2, string assetId2, double amount2, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.SwapAsync(id, clientId1, assetId1, amount1, clientId2, assetId2, amount2, cancellationToken));
        }

        public async Task<MeResponseModel> TransferAsync(string id, string fromClientId, string toClientId, string assetId, int accuracy, double amount, string feeClientId, double feeSizePercentage, double overdraft, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Execute(client => client.TransferAsync(id, fromClientId, toClientId, assetId, accuracy, amount, feeClientId, feeSizePercentage, overdraft, cancellationToken));
        }

        public async Task UpdateBalanceAsync(string id, string clientId, string assetId, double value, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Execute(async (client) =>
            {
                await client.UpdateBalanceAsync(id, clientId, assetId, value, cancellationToken);
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
