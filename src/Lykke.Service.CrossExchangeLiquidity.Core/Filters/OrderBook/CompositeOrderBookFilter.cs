using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public class CompositeOrderBookFilter : IOrderBookFilter
    {
        private readonly IEnumerable<IOrderBookFilter> _filters;

        public CompositeOrderBookFilter(IEnumerable<IOrderBookFilter> filters)
        {
            _filters = filters;
        }

        public bool IsAccepted(Domain.OrderBook.IOrderBook orderBook)
        {
            return _filters.All(f => f.IsAccepted(orderBook));
        }
    }
}
