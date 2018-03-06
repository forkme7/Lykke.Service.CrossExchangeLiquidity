using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface IOrderBookProcessor
    {
        void Process(OrderBook orderBook);
    }
}
