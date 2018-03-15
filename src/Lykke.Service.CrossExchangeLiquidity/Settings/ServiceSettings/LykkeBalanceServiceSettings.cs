using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;
using System;
using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class LykkeBalanceServiceSettings : ILykkeBalanceServiceSettings
    {
        public string ClientId { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public IReadOnlyCollection<string> AssetPairIds { get; set; }
    }
}
