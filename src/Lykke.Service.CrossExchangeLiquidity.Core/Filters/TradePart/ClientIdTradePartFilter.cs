using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.TradePart
{
    public class ClientIdTradePartFilter : ITradePartFilter
    {
        private readonly IClientIdSettings _settings;

        public ClientIdTradePartFilter(IClientIdSettings settings)
        {
            _settings = settings;
        }

        public bool IsAccepted(Domain.LykkeExchange.TradePart tradePart)
        {
            return string.Equals(tradePart.ClientId, _settings.ClientId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
