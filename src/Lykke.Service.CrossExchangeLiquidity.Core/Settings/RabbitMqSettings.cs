using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings
{
    public class RabbitMqSettings
    {
        public string ExchangeName { get; set; }

        public string NameOfEndpoint { get; set; }

        public string ConnectionString { get; set; }
    }
}
