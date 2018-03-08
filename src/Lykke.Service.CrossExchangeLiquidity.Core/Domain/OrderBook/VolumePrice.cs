using Newtonsoft.Json;
using System;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public sealed class VolumePrice
    {
        [JsonProperty("price")]
        public decimal Price { get; }

        [JsonProperty("volume")]
        public decimal Volume { get; }

        public VolumePrice(decimal price, decimal volume)
        {
            Price = price;
            Volume = Math.Abs(volume);
        }
    }
}
