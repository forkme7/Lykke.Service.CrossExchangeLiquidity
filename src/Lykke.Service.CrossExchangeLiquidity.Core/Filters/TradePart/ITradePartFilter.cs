namespace Lykke.Service.CrossExchangeLiquidity.Core.Filters.TradePart
{
    public interface ITradePartFilter
    {
        bool IsAccepted(Domain.LykkeTrade.TradePart tradePart);
    }
}
