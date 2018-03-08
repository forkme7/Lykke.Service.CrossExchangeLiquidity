using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface ITrader
    {
        Task PlaceOrders(IOrderBook orderBook);
    }
}
