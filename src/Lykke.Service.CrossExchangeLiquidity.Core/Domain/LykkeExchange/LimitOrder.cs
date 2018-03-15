using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeExchange
{
    public class LimitOrder
    {
        public Order Order { get; set; }
        public Trade[] Trades { get; set; }
        public IEnumerable<TradePart> TradeParts
        {
            get
            {
                if (Trades == null)
                    return new TradePart[0];

                return Trades.SelectMany(t => new[]
                {
                    new TradePart()
                    {
                        Asset = t.Asset,
                        ClientId = t.ClientId,
                        OrderId = Order.Id,
                        Price = t.Price,
                        Timestamp = t.Timestamp,
                        Volume = t.Volume
                    },
                    new TradePart()
                    {
                        Asset = t.OppositeAsset,
                        ClientId = t.OppositeClientId,
                        OrderId = t.OppositeOrderId,
                        Price = t.Price,
                        Timestamp = t.Timestamp,
                        Volume = t.OppositeVolume
                    }
                });
            }
        }

        public override string ToString()
        {
            return $"{Order}, trades: {string.Join(", ", Trades.Select(x => x.ToString()))}";
        }        
    }
}
