using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class OrderBookSettings
    {
        public RabbitMqSettings RabbitMqOrderBook { get; set; }

        public IReadOnlyCollection<string> TargetPairs { get; set; }
    }
}
