using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class SourceAssetPairIdsSettings : SourceSettings, ISourceAssetPairIdsSettings
    {
        public IReadOnlyCollection<string> AssetPairIds { get; set; }
    }
}
