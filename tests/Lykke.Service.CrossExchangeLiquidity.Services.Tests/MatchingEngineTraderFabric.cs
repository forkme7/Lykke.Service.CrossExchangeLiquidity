using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Moq;
using System;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests
{
    public class MatchingEngineTraderFabric
    {
        public Mock<ILog> Log { get; set; }
        public MatchingEngineTraderSettings  Settings { get; set; }
        public Mock<IMatchingEngineClient> MatchingEngineClient { get; set; }
        public IVolumePriceFilter Filter { get; set; }

        public MatchingEngineTraderFabric()
        {
            Log = new Mock<ILog>();
            Settings = new MatchingEngineTraderSettings(){TimeSpan = new TimeSpan(0,0,0,1), Filter = new CountSettings() { Count = 1 } };
            MatchingEngineClient = new Mock<IMatchingEngineClient>();
            Filter = new TopVolumePriceFilter(Settings.Filter);
        }

        public MatchingEngineTrader CreateMatchingEngineTrader()
        {
            return new MatchingEngineTrader(Log.Object,
                Settings,
                MatchingEngineClient.Object,
                Filter);
        }
    }
}
