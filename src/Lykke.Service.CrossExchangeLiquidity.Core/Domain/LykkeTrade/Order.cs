using System;
using System.Collections.Generic;
using System.Text;
using Lykke.MatchingEngine.Connector.Models;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeTrade
{
    public class Order
    {
        public decimal? Price { get; set; }
        public decimal RemainingVolume { get; set; }
        public DateTime? LastMatchTime { get; set; }
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string AssetPairId { get; set; }
        public string ClientId { get; set; }
        public decimal Volume { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Registered { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, ExternalId: {ExternalId}, Price: {Price}, Volume: {Volume}, Remaining: {RemainingVolume}, Status: {Status}";
        }
    }
}
