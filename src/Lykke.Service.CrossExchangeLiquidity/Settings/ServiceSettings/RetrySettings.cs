using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class RetrySettings : IRetrySettings
    {
        public int Times { get; set; }

        public int Multiplier { get; set; }

        public TimeSpan TimeSpan { get; set; }
    }
}
