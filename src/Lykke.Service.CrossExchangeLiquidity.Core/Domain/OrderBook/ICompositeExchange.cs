using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public interface ICompositeExchange : IReadOnlyDictionary<string, ICompositeOrderBook>
    {
        void AddOrUpdateOrderBook(string source, IOrderBook orderBook);
    }
}
