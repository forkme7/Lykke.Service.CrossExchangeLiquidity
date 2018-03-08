using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public class CompositeExchange : Dictionary<string, ICompositeOrderBook>, ICompositeExchange
    {
        public void AddOrUpdateOrderBook(string source, IOrderBook orderBook)
        {
            if (!TryGetValue(orderBook.AssetPairId, out var compositeOrderBook))
            {
                compositeOrderBook = new CompositeOrderBook(orderBook.AssetPairId);
                this[orderBook.AssetPairId] = compositeOrderBook;
            }
            compositeOrderBook.AddOrUpdateOrderBook(source, orderBook);
        }
    }
}
