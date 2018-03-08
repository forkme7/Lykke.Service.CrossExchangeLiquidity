namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings
{
    public interface IRetrySettings : ITimeSpanSettings
    {
        int Times { get; }

        int Multiplier { get; }
    }
}
