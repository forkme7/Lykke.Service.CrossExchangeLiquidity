using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeExchange
{
    public class LimitOrderMessage
    {
        public LimitOrder[] Orders { get; set; }
    }
}
