using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface IOrderBookProcessor
    {
        Task Process(OrderBook orderBook);
    }
}
