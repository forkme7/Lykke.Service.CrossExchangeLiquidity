using System;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange
{
    public class MatchingEngineRetryAdapterSettings : IMatchingEngineRetryAdapterSettings
    {
        public int Times { get; set; }

        public int Multiplier { get; set; }

        public TimeSpan TimeSpan { get; set; }
    }
}
