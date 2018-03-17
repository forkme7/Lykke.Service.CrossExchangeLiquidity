using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeTrade
{
    public class LimitOrderMessage
    {
        public LimitOrder[] Orders { get; set; }
    }
}
