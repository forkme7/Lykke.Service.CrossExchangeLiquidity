using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeExchange
{
    public interface ITradeFilter
    {
        IEnumerable<VolumePrice> GetAsks(IOrderBook orderBook);

        IEnumerable<VolumePrice> GetBids(IOrderBook orderBook);
    }
}
