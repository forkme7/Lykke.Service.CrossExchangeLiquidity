using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public class SourceFilter : IOrderBookFilter
    {
        private readonly ISourceSettings _settings;

        public SourceFilter(ISourceSettings settings)
        {
            _settings = settings;
        }

        public bool IsAccepted(Domain.OrderBook.OrderBook orderBook)
        {
            return string.Equals(orderBook.Source, _settings.Source, StringComparison.OrdinalIgnoreCase);
        }
    }
}
