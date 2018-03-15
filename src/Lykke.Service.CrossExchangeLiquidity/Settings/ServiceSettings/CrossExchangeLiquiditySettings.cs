using Lykke.Service.CrossExchangeLiquidity.Core.Settings.ExternalExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class CrossExchangeLiquiditySettings
    {
        public DbSettings Db { get; set; }

        public OrderBookSettings OrderBook { get; set; }

        public MatchingEngineTraderSettings MatchingEngineTrader { get; set; }

        [HttpCheck("api/isalive")]
        public string AssetsServiceUrl { get; set; }

        [HttpCheck("api/isalive")]
        public string BalancesServiceUrl { get; set; }

        public ExternalBalanceServicesSettings ExternalBalance { get; set; }

        public LykkeBalanceServiceSettings LykkeBalance { get; set; }
    }
}
