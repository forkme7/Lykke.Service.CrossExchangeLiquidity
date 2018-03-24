namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.Orders
{
    public class LykkeMultiOrderItemModel
    {
        public string Id { get; set; }
        public OrderAction OrderAction { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public LykkeLimitOrderFeeModel Fee { get; set; }
    }
}
