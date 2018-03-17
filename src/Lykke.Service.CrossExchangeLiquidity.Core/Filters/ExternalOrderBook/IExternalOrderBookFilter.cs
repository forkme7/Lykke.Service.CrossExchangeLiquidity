namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook
{
    public interface IExternalOrderBookFilter
    {
        bool IsAccepted(Domain.ExternalOrderBook.IExternalOrderBook orderBook);
    }
}
