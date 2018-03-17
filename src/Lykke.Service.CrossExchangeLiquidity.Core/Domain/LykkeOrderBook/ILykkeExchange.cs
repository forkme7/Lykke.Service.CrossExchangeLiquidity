using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook
{
    public interface ILykkeExchange
    {
        void AddOrUpdate(LykkeOrderBook orderBook);

        OrderBook GetOrderBook(string assetPairId);
    }
}
