using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalExchange;

namespace Lykke.Service.CrossExchangeLiquidity.AzureRepositories.AssetBalance
{
    public class AssetBalanceRepository : IAssetBalanceRepository
    {
        private readonly INoSQLTableStorage<AssetBalanceEntity> _storage;

        public AssetBalanceRepository(INoSQLTableStorage<AssetBalanceEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IAssetBalance> GetAsync(string source, string assetId)
        {
            return await _storage.GetDataAsync(AssetBalanceEntity.GetPartitionKey(source), AssetBalanceEntity.GetRowKey(assetId));
        }

        public async Task<IEnumerable<IAssetBalance>> GetAsync(string source)
        {
            return await _storage.GetDataAsync(AssetBalanceEntity.GetPartitionKey(source));
        }

        public async Task<IEnumerable<IAssetBalance>> GetAsync(IEnumerable<string> sources)
        {
            return await _storage.GetDataAsync(sources.Select(AssetBalanceEntity.GetPartitionKey));
        }

        public async Task InsertAsync(IAssetBalance assetBalance)
        {
            await _storage.InsertAsync(new AssetBalanceEntity(assetBalance));
        }

        public async Task InsertAsync(IEnumerable<IAssetBalance> assetBalances)
        {
            await _storage.InsertAsync(assetBalances.Select(b=> new AssetBalanceEntity(b)));
        }

        public async Task ReplaceAsync(IAssetBalance assetBalance)
        {
            await _storage.ReplaceAsync(new AssetBalanceEntity(assetBalance));
        }

        public async Task InsertOrReplaceAsync(IAssetBalance assetBalance)
        {
            await _storage.InsertOrReplaceAsync(new AssetBalanceEntity(assetBalance));
        }
    }
}
