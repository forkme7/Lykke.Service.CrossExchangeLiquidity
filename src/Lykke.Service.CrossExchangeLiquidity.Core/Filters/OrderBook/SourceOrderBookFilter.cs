using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public class SourceOrderBookFilter : IOrderBookFilter
    {
        private readonly ISourceSettings _settings;

        public SourceOrderBookFilter(ISourceSettings settings)
        {
            _settings = settings;
        }

        public bool IsAccepted(Domain.OrderBook.IOrderBook orderBook)
        {
            return string.Equals(orderBook.Source, _settings.Source, StringComparison.OrdinalIgnoreCase);
        }
    }
}
