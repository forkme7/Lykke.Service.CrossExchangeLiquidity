using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook
{
    public class CompositeExchange : Dictionary<string, ICompositeOrderBook>, ICompositeExchange
    {
        public void AddOrUpdateOrderBook(IExternalOrderBook orderBook)
        {
            if (!TryGetValue(orderBook.AssetPairId, out var compositeOrderBook))
            {
                compositeOrderBook = new CompositeOrderBook(orderBook.AssetPairId);
                this[orderBook.AssetPairId] = compositeOrderBook;
            }
            compositeOrderBook.AddOrUpdateOrderBook(orderBook);
        }
    }
}
