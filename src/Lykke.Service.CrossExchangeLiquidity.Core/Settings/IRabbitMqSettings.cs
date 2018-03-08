namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings
{
    public interface IRabbitMqSettings
    {
        string ExchangeName { get; }

        string NameOfEndpoint { get; }

        string ConnectionString { get; }
    }
}
