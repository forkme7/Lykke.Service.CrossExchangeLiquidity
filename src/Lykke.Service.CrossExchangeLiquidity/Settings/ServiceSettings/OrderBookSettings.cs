using System.Collections.Generic;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class OrderBookSettings
    {
        public RabbitMqSettings Source { get; set; }

        public IReadOnlyCollection<SourceAssetPairIdsSettings> Filter { get; set; }
    }
}
