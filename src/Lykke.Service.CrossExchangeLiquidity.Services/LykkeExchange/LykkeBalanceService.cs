using Autofac;
using Common;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange
{
    public class LykkeBalanceService : ILykkeBalanceService, IStartable, IStopable
    {
        private readonly IBalancesClient _balancesClient;
        private readonly ILykkeBalanceServiceSettings _settings;
        private readonly ConcurrentDictionary<string, decimal> _balances;
        private readonly IAssetPairDictionary _assetPairDictionary;
        private string[] _assetIds;
        private Timer _timer;

        public LykkeBalanceService(IBalancesClient balancesClient,
            IAssetPairDictionary assetPairDictionary,
            ILykkeBalanceServiceSettings settings)
        {
            _balancesClient = balancesClient;
            _assetPairDictionary = assetPairDictionary;
            _settings = settings;
            _balances = new ConcurrentDictionary<string, decimal>();
        }

        private string[] GetSupportedAssetIds(IEnumerable<string> assetPairIds)
        {
            var assetIds = new List<string>();
            foreach (string assetPairId in assetPairIds)
            {                
                if (!_assetPairDictionary.ContainsKey(assetPairId))
                {
                    throw new ArgumentOutOfRangeException($"Provided AssetPairId {assetPairId} is not supported by Lykke Exchange.");
                }
                AssetPair assetPair = _assetPairDictionary[assetPairId];
                assetIds.Add(assetPair.BaseAssetId);
                assetIds.Add(assetPair.QuotingAssetId);
            }
            return assetIds.ToArray();
        }

        private async Task GetFromServerAsync()
        {
            IEnumerable<ClientBalanceResponseModel> response =
                await _balancesClient.GetClientBalances(_settings.ClientId);

            _balances.Clear();
            foreach (ClientBalanceResponseModel model in response)
            {
                if (!_assetIds.Contains(model.AssetId))
                {
                    continue;
                }

                _balances.AddOrUpdate(model.AssetId, (decimal)model.Balance, (k, v) => (decimal)model.Balance);
            }
        }

        public async Task AddAssetAsync(string assetId, decimal value)
        {
            if (!_assetIds.Contains(assetId))
            {
                throw new ArgumentOutOfRangeException($"Provided assetId {assetId} is not supported by LykkeExchange.");
            }

            _balances.AddOrUpdate(assetId, value, (k, v) => v + value);

            await Task.CompletedTask;
        }

        public decimal GetAssetBalance(string assetId)
        {
            return _balances[assetId];
        }

        public void Start()
        {
            //todo: use StartupManager
            _assetIds = GetSupportedAssetIds(_settings.AssetPairIds);
            Task.Run(async () => await GetFromServerAsync()).RunSynchronously();

            _timer = new Timer(_settings.TimeSpan.TotalMilliseconds);
            _timer.Elapsed += async (sender, e) => await GetFromServerAsync();
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Stop()
        {
            _timer?.Stop();
        }
    }
}
