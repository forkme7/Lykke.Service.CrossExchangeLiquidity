using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Moq;
using System;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests
{
    public class MatchingEngineTraderFabric
    {
        public Mock<ILog> Log { get; set; }
        public MatchingEngineTraderSettings  Settings { get; set; }
        public Mock<IMatchingEngineClient> MatchingEngineClient { get; set; }
        public Mock<IBestPriceEvaluator> BestPriceEvaluator { get; set; }
        public IVolumePriceFilter Filter { get; set; }

        public const int Count = 1;

        public MatchingEngineTraderFabric()
        {
            Log = new Mock<ILog>();
            Settings = new MatchingEngineTraderSettings()
            {
                TimeSpan = new TimeSpan(0,0,0,1),
            };
            MatchingEngineClient = new Mock<IMatchingEngineClient>();
            BestPriceEvaluator = new Mock<IBestPriceEvaluator>();
            Filter = new TopVolumePriceFilter(Count);
        }

        public MatchingEngineTrader CreateMatchingEngineTrader()
        {
            return new MatchingEngineTrader(Log.Object,
                Settings,
                MatchingEngineClient.Object,
                BestPriceEvaluator.Object,
                Filter);
        }
    }
}
