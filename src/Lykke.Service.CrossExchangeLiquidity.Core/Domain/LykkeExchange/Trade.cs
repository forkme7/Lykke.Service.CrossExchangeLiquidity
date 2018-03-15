using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeExchange
{
    public class Trade
    {
        public string ClientId { get; set; }
        public string Asset { get; set; }
        public decimal Volume { get; set; }
        public decimal? Price { get; set; }
        public DateTime Timestamp { get; set; }
        public string OppositeOrderId { get; set; }
        public string OppositeOrderExternalId { get; set; }
        public string OppositeAsset { get; set; }
        public string OppositeClientId { get; set; }
        public decimal OppositeVolume { get; set; }

        public override string ToString()
        {
            return $"Time: {Timestamp}, Price: {Price}, Volume: {Volume}, Asset: {Asset}, ClientId: {ClientId}, OppClientId: {OppositeClientId}";
        }
    }
}
