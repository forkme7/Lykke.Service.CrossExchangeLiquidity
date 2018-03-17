using System.Reflection;
using Common.Log;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using System.Threading.Tasks;
using Domain = Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Services.OrderBook
{
    public class ExternalOrderBookProcessor : IMessageProcessor<Domain.ExternalOrderBook>
    {
        private readonly ILog _log;
        private readonly IExternalOrderBookFilter _externalOrderBookFilter;
        private readonly Domain.ICompositeExchange _compositeExchange;
        private readonly ILykkeTrader _trader;

        public ExternalOrderBookProcessor(ILog log,
            IExternalOrderBookFilter externalOrderBookFilter,
            Domain.ICompositeExchange compositeExchange,
            ILykkeTrader trader)
        {
            _log = log;
            _externalOrderBookFilter = externalOrderBookFilter;
            _compositeExchange = compositeExchange;
            _trader = trader;
        }

        public async Task ProcessAsync(Domain.ExternalOrderBook orderBook)
        {
            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, $">> {orderBook}");

            if (!_externalOrderBookFilter.IsAccepted(orderBook))
            {
                await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name,
                    $"External order book is skipped: {orderBook}");
                return;
            }

            _compositeExchange.AddOrUpdateOrderBook(orderBook);
            _compositeExchange.TryGetValue(orderBook.AssetPairId, out var compositeOrderBook);

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, $"Composite order book is {orderBook}");

            await _trader.PlaceOrdersAsync(compositeOrderBook);

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, "<<");
        }
    }
}
