using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Moq;
using System;
using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests.LykkeExchange
{
    public class MatchingEngineTraderFactory
    {
        public Mock<ILog> Log { get; set; }
        public Mock<IMatchingEngineTraderSettings> Settings { get; set; }
        public Mock<IMatchingEngineClient> MatchingEngineClient { get; set; }
        public Mock<IBestPriceEvaluator> BestPriceEvaluator { get; set; }
        public Mock<IVolumePriceFilter> Filter { get; set; }

        public const int Count = 1;

        public MatchingEngineTraderFactory()
        {
            Log = new Mock<ILog>();

            Settings = new Mock<IMatchingEngineTraderSettings>();
            Settings.Setup(s => s.TimeSpan).Returns(new TimeSpan(0, 0, 0, 1));

            MatchingEngineClient = new Mock<IMatchingEngineClient>();

            BestPriceEvaluator = new Mock<IBestPriceEvaluator>();
            BestPriceEvaluator.Setup(e => e.GetMinAsk(It.IsAny<string>())).Returns(0);
            BestPriceEvaluator.Setup(e => e.GetMaxBid(It.IsAny<string>())).Returns(decimal.MaxValue);

            Filter = new Mock<IVolumePriceFilter>();
            Filter.Setup(f => f.GetAsks(It.IsAny<string>(), It.IsAny<IEnumerable<SourcedVolumePrice>>()))
                .Returns<string, IEnumerable<SourcedVolumePrice>>((assetPairId, asks)=> asks);
            Filter.Setup(f => f.GetBids(It.IsAny<string>(), It.IsAny<IEnumerable<SourcedVolumePrice>>()))
                .Returns<string, IEnumerable<SourcedVolumePrice>>((assetPairId, bids) => bids);
        }

        public MatchingEngineTrader CreateMatchingEngineTrader()
        {
            return new MatchingEngineTrader(Log.Object,
                Settings.Object,
                MatchingEngineClient.Object,
                BestPriceEvaluator.Object,
                Filter.Object);
        }
    }
}
