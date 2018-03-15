namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.TradePart
{
    public interface ITradePartFilter
    {
        bool IsAccepted(Domain.LykkeExchange.TradePart tradePart);
    }
}
