namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook
{
    public interface IOrderBookFilter
    {
        bool IsAccepted(Domain.OrderBook.IOrderBook orderBook);
    }
}
