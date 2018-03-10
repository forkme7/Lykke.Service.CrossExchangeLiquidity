using Lykke.Service.CrossExchangeLiquidity.Core.Settings;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class CrossExchangeLiquiditySettings
    {
        public DbSettings Db { get; set; }

        public OrderBookSettings OrderBook { get; set; }

        public LykkeExchangeSettings LykkeExchange { get; set; }
    }
}
