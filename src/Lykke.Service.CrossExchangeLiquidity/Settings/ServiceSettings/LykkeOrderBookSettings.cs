using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class LykkeOrderBookSettings
    {
        public RabbitMqSettings Source { get; set; }

        public IReadOnlyCollection<string> Filter { get; set; }
    }
}
