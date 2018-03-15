using Common.Log;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.TradePart;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Domain = Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange
{
    public class LimitOrderMessageProcessor : IMessageProcessor<LimitOrderMessage>
    {
        private readonly ILog _log;
        private readonly ITradePartFilter _filter;
        private readonly ILykkeBalanceService _lykkeBalanceService;
        private readonly Domain.ICompositeExchange _compositeExchange;

        public LimitOrderMessageProcessor(ILog log,
            ITradePartFilter filter,
            ILykkeBalanceService lykkeBalanceService,
            Domain.ICompositeExchange compositeExchange)
        {
            _log = log;
            _filter = filter;
            _lykkeBalanceService = lykkeBalanceService;
            _compositeExchange = compositeExchange;
        }

        public async Task ProcessAsync(LimitOrderMessage limitOrderMessage)
        {
            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, $">>");

            TradePart[] tradeParts = limitOrderMessage.Orders
                .SelectMany(o => o.TradeParts)
                .Where(t => _filter.IsAccepted(t))
                .ToArray();           

            foreach (TradePart tradePart in tradeParts)
            {
                await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, $"Selected part of trade to process: {tradePart}");
                await _lykkeBalanceService.AddAssetAsync(tradePart.Asset, tradePart.Volume);
                //todo: Make external order.
            }

            await _log.WriteInfoAsync(GetType().Name, MethodBase.GetCurrentMethod().Name, "<<");
        }
    }
}
