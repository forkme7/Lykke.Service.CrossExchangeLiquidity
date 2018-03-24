using System.Collections.Generic;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Models.OrderBook
{
    public interface IOrderBookProvider
    {
        IEnumerable<OrderBookModel> Get(string source, ICompositeOrderBook compositeOrderBook);
        IEnumerable<OrderBookModel> Get(ICompositeOrderBook compositeOrderBook);
        IEnumerable<OrderBookModel> Get(IReadOnlyDictionary<string, ICompositeOrderBook> compositeExchange);

        IEnumerable<OrderBookModel> Get(string source,
            IReadOnlyDictionary<string, ICompositeOrderBook> compositeExchange);
        IEnumerable<OrderBookModel> Get(string source, IEnumerable<Core.Domain.LykkeOrderBook.OrderBook> orderBooks);
        IEnumerable<OrderBookModel> Get(string source, Core.Domain.LykkeOrderBook.OrderBook orderBook);

    }
}
