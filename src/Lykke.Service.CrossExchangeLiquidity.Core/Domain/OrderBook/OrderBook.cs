using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public sealed class OrderBook : IOrderBook
    {
        [JsonProperty("source")]
        public string Source { get; }

        [JsonProperty("asset")]
        public string AssetPairId { get; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; }

        [JsonProperty("asks")]
        public IEnumerable<VolumePrice> Asks { get; }

        [JsonProperty("bids")]
        public IEnumerable<VolumePrice> Bids { get; }

        public OrderBook(string source, 
            string assetPairId, 
            IEnumerable<VolumePrice> bids, 
            IEnumerable<VolumePrice> asks, 
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
