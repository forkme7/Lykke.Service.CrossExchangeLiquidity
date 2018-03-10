using System;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings
{
    public class LykkeExchangeSettings: ClientIdSettings, ILykkeExchangeSettings
    {
        public IpEndpointSettings IpEndpoint { get; set; }

        public RetrySettings Retry { get; set; }

        public CountSettings Filter { get; set; }

        public TimeSpan TimeSpan { get; set; }
    }
}
