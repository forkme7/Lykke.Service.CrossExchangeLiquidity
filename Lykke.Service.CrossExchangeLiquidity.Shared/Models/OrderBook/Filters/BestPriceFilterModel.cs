namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook.Filters
{
    public class BestPriceFilterModel : VolumePriceFilterModel
    {
        public decimal MinAsk { get; set; }

        public decimal MaxBid { get; set; }
    }
}
