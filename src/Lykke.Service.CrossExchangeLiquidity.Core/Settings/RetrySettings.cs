using System;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings
{
    public class RetrySettings : IRetrySettings
    {
        public int Times { get; set; }

        public int Multiplier { get; set; }

        public TimeSpan TimeSpan { get; set; }
    }
}
