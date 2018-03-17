using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalBalance;

namespace Lykke.Service.CrossExchangeLiquidity.AzureRepositories.AssetBalance
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class AssetBalanceEntity : AzureTableEntity, IAssetBalance
    {
        private decimal _value;
        public string Source => PartitionKey;
        public string AssetId => RowKey;
        public decimal Value
        {
            get => _value;
            set
            {
                _value = value;
                MarkValueTypePropertyAsDirty(nameof(Value));
            }
        } 

        public AssetBalanceEntity()
        {
        }

        public AssetBalanceEntity(IAssetBalance assetBalance)
        {
            PartitionKey = GetPartitionKey(assetBalance.Source);
            RowKey = GetRowKey(assetBalance.AssetId);
            Value = assetBalance.Value;
        }

        internal static string GetPartitionKey(string source)
            => source;

        internal static string GetRowKey(string assetId)
            => assetId;
    }
}
