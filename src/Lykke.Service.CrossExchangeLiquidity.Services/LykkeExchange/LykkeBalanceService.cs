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
        private Timer _timer;

        public LykkeBalanceService(IBalancesClient balancesClient,
            ILykkeBalanceServiceSettings settings)
        {
            _balancesClient = balancesClient;
            _settings = settings;
            _balances = new ConcurrentDictionary<string, decimal>();
        }

        private async Task GetFromServerAsync()
        {
            IEnumerable<ClientBalanceResponseModel> response =
                await _balancesClient.GetClientBalances(_settings.ClientId);

            _balances.Clear();
            foreach (ClientBalanceResponseModel model in response)
            {
                if (!_settings.AssetIds.Contains(model.AssetId))
                {
                    continue;
                }

                _balances.AddOrUpdate(model.AssetId, (decimal)model.Balance, (k, v) => (decimal)model.Balance);
            }
        }

        public async Task AddAssetAsync(string assetId, decimal value)
        {
            if (!_settings.AssetIds.Contains(assetId))
            {
                throw new ArgumentOutOfRangeException($"Provided assetId {assetId} is not supported by LykkeExchange.");
            }

            _balances.AddOrUpdate(assetId, value, (k, v) => v + value);

            await Task.CompletedTask;
        }

        public decimal GetAssetBalance(string assetId)
        {
            if (_balances.TryGetValue(assetId, out var balance))
            {
                return balance;
            }

            return 0;
        }

        public void Start()
        {
            Task.Run(async () => await GetFromServerAsync()).Wait();
            //todo: use StartupManager

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
