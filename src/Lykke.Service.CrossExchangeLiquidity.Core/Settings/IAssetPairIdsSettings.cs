using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings
{
    public interface IAssetPairIdsSettings
    {
        IReadOnlyCollection<string> AssetPairIds { get; set; }
    }
}
