using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings.OrderBook
{
    public interface ISourceAssetPairIdsSettings
    {
        string Source { get; set; }

        IReadOnlyCollection<string> AssetPairIds { get; set; }
    }
}
