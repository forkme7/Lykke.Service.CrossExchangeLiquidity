namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeOrderBook
{
    public interface ILykkeOrderBookFilter
    {
        bool IsAccepted(Domain.LykkeOrderBook.LykkeOrderBook orderBook);
    }
}
