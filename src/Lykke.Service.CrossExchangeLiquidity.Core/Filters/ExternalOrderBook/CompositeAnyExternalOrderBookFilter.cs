using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook
{
    public class CompositeAnyExternalOrderBookFilter : IExternalOrderBookFilter
    {
        private readonly IEnumerable<IExternalOrderBookFilter> _filters;

        public CompositeAnyExternalOrderBookFilter(IEnumerable<IExternalOrderBookFilter> filters)
        {
            _filters = filters;
        }

        public bool IsAccepted(Domain.ExternalOrderBook.IExternalOrderBook orderBook)
        {
            return _filters.Any(f => f.IsAccepted(orderBook));
        }
    }
}
