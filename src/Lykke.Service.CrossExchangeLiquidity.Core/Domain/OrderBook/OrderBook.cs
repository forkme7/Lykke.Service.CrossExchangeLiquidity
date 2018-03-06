using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public sealed class OrderBook
    {
        [JsonProperty("source")]
        public string Source { get; }

        [JsonProperty("asset")]
        public string AssetPairId { get; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; }

        [JsonProperty("asks")]
        public IReadOnlyCollection<VolumePrice> Asks { get; }

        [JsonProperty("bids")]
        public IReadOnlyCollection<VolumePrice> Bids { get; }

        public OrderBook(string source, string assetPairId, IReadOnlyCollection<VolumePrice> bids, IReadOnlyCollection<VolumePrice> asks, DateTime timestamp)
        {
            Source = source;
            AssetPairId = assetPairId;
            Asks = asks;
            Bids = bids;
            Timestamp = timestamp;
        }
    }
}
