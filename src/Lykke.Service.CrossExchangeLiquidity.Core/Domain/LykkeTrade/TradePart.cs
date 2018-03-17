using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeTrade
{
    public class TradePart
    {
        public string OrderId { get; set; }
        public string ClientId { get; set; }
        public string Asset { get; set; }
        public decimal Volume { get; set; }
        public decimal? Price { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"Time: {Timestamp}, OrderId: {OrderId}, Price: {Price}, Volume: {Volume}, Asset: {Asset}, ClientId: {ClientId}";
        }
    }
}
