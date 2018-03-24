using Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook.Filters;

namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook
{
    public class FilteredCompositeOrderBookModel: CompositeOrderBookModel
    {
        public VolumePriceFilterModel Filter { get; set; }
    }
}
