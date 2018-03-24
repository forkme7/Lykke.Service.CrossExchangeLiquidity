using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook
{
    public class OrderBookModel
    {
        public string Source { get; set; }

        public string AssetPairId { get; set; }

        public IEnumerable<VolumePriceModel> Asks { get; set; }

        public IEnumerable<VolumePriceModel> Bids { get; set; }
    }
}
