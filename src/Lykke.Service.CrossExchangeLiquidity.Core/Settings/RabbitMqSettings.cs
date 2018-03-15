namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings
{
    public class RabbitMqSettings : IRabbitMqSettings
    {
        public string ExchangeName { get; set; }

        public string NameOfEndpoint { get; set; }

        public string ConnectionString { get; set; }
    }
}
