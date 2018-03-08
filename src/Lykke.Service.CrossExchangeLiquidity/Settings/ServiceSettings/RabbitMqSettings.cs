using Lykke.Service.CrossExchangeLiquidity.Core.Settings;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class RabbitMqSettings : IRabbitMqSettings
    {
        public string ExchangeName { get; set; }

        public string NameOfEndpoint { get; set; }

        public string ConnectionString { get; set; }
    }
}
