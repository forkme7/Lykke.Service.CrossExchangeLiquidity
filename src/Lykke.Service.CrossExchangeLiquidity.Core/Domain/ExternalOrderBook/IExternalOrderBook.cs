using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook
{
    public  interface IExternalOrderBook
    {
        string Source { get; }

        string AssetPairId { get; }

        IEnumerable<ExternalVolumePrice> Asks { get; }

        IEnumerable<ExternalVolumePrice> Bids { get; }
    }
}
