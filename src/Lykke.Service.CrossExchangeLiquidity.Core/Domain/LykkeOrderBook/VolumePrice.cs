using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook
{
    public class VolumePrice
    {
        public decimal Price { get;}

        public decimal Volume { get; }

        public VolumePrice(decimal price, decimal volume)
        {
            Price = price;
            Volume = volume;
        }
    }
}
