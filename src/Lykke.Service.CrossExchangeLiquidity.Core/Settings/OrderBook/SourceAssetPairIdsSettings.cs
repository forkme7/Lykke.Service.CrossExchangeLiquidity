using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings.OrderBook
{
    public class SourceAssetPairIdsSettings : ISourceAssetPairIdsSettings
    {
        public string Source { get; set; }

        public IReadOnlyCollection<string> AssetPairIds { get; set; }
    }
}
