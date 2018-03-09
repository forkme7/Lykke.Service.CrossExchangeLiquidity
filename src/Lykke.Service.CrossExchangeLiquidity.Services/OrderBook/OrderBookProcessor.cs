using System.Reflection;
using Common.Log;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using System.Threading.Tasks;
using Domain = Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Services.OrderBook
{
    public class OrderBookProcessor : IOrderBookProcessor
    {
        private readonly ILog _log;
        private readonly IOrderBookFilter _orderBookFilter;
        private readonly Domain.ICompositeExchange _compositeExchange;
        private readonly ITrader _trader;

        public OrderBookProcessor(ILog log, 
            IOrderBookFilter orderBookFilter,
            Domain.ICompositeExchange compositeExchange,
            ITrader trader)
        {
            _log = log;
            _orderBookFilter = orderBookFilter;
            _compositeExchange = compositeExchange;
            _trader = trader;
        }

        public async Task Process(Domain.OrderBook orderBook)
        {
            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, $">> {orderBook}");

            if (!_orderBookFilter.IsAccepted(orderBook))
            {
                await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, $"Order book is skipped: {orderBook}");
                return;
            }

            _compositeExchange.AddOrUpdateOrderBook(orderBook.Source, orderBook);
            _compositeExchange.TryGetValue(orderBook.AssetPairId, out var compositeOrderBook);

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, $"Composite order book is {orderBook}");

            await _trader.PlaceOrders(compositeOrderBook);

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, "<<");
        }
    }
}
