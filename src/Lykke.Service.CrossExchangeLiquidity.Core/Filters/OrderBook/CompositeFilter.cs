using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public class CompositeFilter : IOrderBookFilter
    {
        private readonly IEnumerable<IOrderBookFilter> _filters;

        public CompositeFilter(IEnumerable<IOrderBookFilter> filters)
        {
            _filters = filters;
        }

        public bool IsAccepted(Domain.OrderBook.OrderBook orderBook)
        {
            return _filters.All(f => f.IsAccepted(orderBook));
        }
    }
}
