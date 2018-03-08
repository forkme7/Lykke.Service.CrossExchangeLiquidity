using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class LykkeExchangeSettings: ClientIdSettings, ILykkeExchangeSettings
    {
        public IpEndpointSettings IpEndpoint { get; set; }

        public RetrySettings Retry { get; set; }

        public CountSettings Filter { get; set; }

        public TimeSpan TimeSpan { get; set; }
    }
}
