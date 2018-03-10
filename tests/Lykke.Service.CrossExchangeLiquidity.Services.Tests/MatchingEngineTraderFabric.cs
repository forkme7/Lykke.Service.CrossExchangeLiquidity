using System;
using System.Collections.Generic;
using System.Text;
using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Moq;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests
{
    public class MatchingEngineTraderFabric
    {
        public Mock<ILog> Log { get; set; }
        public LykkeExchangeSettings  Settings { get; set; }
        public Mock<IMatchingEngineClient> MatchingEngineClient { get; set; }
        public ITradeFilter Filter { get; set; }

        public MatchingEngineTraderFabric()
        {
            Log = new Mock<ILog>();
            Settings = new LykkeExchangeSettings(){TimeSpan = new TimeSpan(0,0,0,1), Filter = new CountSettings() { Count = 1 } };
            MatchingEngineClient = new Mock<IMatchingEngineClient>();
            Filter = new TopFilter(Settings.Filter);
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
