using Newtonsoft.Json;
using System;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook
{
    public sealed class ExternalVolumePrice
    {
        [JsonProperty("price")]
        public decimal Price { get; }

        [JsonProperty("volume")]
        public decimal Volume { get; }

        public ExternalVolumePrice(decimal price, decimal volume)
        {
            Price = price;
            Volume = Math.Abs(volume);
        }
    }
}
