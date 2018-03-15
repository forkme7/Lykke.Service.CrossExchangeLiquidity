namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange
{
    public interface IMatchingEngineRetryAdapterSettings : ITimeSpanSettings
    {
        int Times { get; }

        int Multiplier { get; }
    }
}
