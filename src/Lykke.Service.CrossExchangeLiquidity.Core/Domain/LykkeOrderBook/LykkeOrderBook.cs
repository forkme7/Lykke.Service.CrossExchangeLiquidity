using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook
{
    public class LykkeOrderBook
    {
        [JsonProperty("assetPair")]
        public string AssetPairId { get; set; }

        public bool IsBuy { get; set; }

        public DateTime Timestamp { get; set; }

        public IReadOnlyList<LykkeVolumePrice> Prices { get; set; }
    }
}
