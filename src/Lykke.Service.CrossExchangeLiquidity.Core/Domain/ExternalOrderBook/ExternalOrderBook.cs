using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook
{
    public sealed class ExternalOrderBook : IExternalOrderBook
    {
        [JsonProperty("source")]
        public string Source { get; }

        [JsonProperty("asset")]
        public string AssetPairId { get; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; }

        [JsonProperty("asks")]
        public IEnumerable<ExternalVolumePrice> Asks { get; }

        [JsonProperty("bids")]
        public IEnumerable<ExternalVolumePrice> Bids { get; }

        public ExternalOrderBook(string source, 
            string assetPairId, 
            IEnumerable<ExternalVolumePrice> bids, 
            IEnumerable<ExternalVolumePrice> asks, 
            DateTime timestamp)
        {
            Source = source;
            AssetPairId = assetPairId;
            Asks = asks;
            Bids = bids;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            return GetType().Name + " " + 
                   $"Source = {Source} " +
                   $"AssetPairId = {AssetPairId} " +
                   $"Timestamp = {Timestamp} " +
                   $"TopAsk = {Asks.FirstOrDefault()} " +
                   $"TopBids = {Bids.FirstOrDefault()}";
        }
    }
}
