using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange
{
    public interface ILykkeBalanceServiceSettings: IClientIdSettings, ITimeSpanSettings
    {
        IReadOnlyCollection<string> AssetIds { get; }
    }
}
