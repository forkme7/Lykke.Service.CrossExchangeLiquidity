using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings
{
    public class AssetPairIdsSettings : IAssetPairIdsSettings
    {
        public IReadOnlyCollection<string> AssetPairIds { get; set; }
    }
}
