using Autofac;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalBalance;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.ExternalExchange;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Services.ExternalExchange
{
    public class ExternalBalanceService : IExternalBalanceService, IStartable
    {
        private readonly IAssetBalanceRepository _repository;
        private readonly IExternalBalanceServicesSettings _settings;
        private ReadOnlyDictionary<string, ConcurrentDictionary<string, decimal>> _balances;

        public ExternalBalanceService(IAssetBalanceRepository repository,
            IExternalBalanceServicesSettings settings)
        {
            _repository = repository;
            _settings = settings;            
        }

        private async Task InitializeAsync()
        {
            _balances = new ReadOnlyDictionary<string, ConcurrentDictionary<string, decimal>>(
                _settings.Balances.ToDictionary(p => p.Source,
                    p => new ConcurrentDictionary<string, decimal>(
                        p.AssetValues.ToDictionary(a => a.Asset, a => a.Value))));

            IAssetBalance[] assetBalances = (await _repository.GetAsync(_balances.Keys)).ToArray();

            foreach (IAssetBalance assetBalance in assetBalances)
            {
                ConcurrentDictionary<string, decimal> externalAssetValues = _balances[assetBalance.Source];

                if (!externalAssetValues.ContainsKey(assetBalance.AssetId))
                {
                    continue;
                }

                _balances[assetBalance.Source][assetBalance.AssetId] = assetBalance.Value;
            }
        }

        public async Task AddAssetAsync(string source, string assetId, decimal value)
        {
            var newValue = _balances[source].AddOrUpdate(assetId, 
                k => throw new ArgumentOutOfRangeException(
                    $"Provided assetId {assetId} is not supported by exchange {source}."),
                (k, v) => v + value);
            
            await _repository.InsertOrReplaceAsync(new AssetBalance(source, assetId) {Value = newValue});
        }

        public decimal GetAssetBalance(string source, string assetId)
        {
            return _balances[source][assetId];
        }

        public void Start()
        {
            //todo: use StartupManager

            Task.Run(async () => await InitializeAsync()).Wait();
        }
    }
}
