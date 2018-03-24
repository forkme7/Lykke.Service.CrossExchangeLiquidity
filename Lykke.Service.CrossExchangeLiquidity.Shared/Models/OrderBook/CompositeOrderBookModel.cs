using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook
{
    public class CompositeOrderBookModel
    {
        public string AssetPairId { get; set; }

        public IEnumerable<SourcedVolumePriceModel> Asks { get; set; }

        public IEnumerable<SourcedVolumePriceModel> Bids { get; set; }
    }
}
