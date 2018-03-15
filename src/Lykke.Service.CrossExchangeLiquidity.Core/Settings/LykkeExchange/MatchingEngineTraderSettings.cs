using System;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange
{
    public class MatchingEngineTraderSettings: ClientIdSettings, IMatchingEngineTraderSettings
    {
        public IpEndpointSettings IpEndpoint { get; set; }

        public MatchingEngineRetryAdapterSettings Retry { get; set; }

        public CountSettings Filter { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public RabbitMqSettings LimitOrders { get; set; }
    }
}
