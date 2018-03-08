namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public interface ICompositeOrderBook : IOrderBook
    {
        void AddOrUpdateOrderBook(string source, IOrderBook orderBook);
    }
}
