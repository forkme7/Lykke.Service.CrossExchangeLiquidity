using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using System.Threading.Tasks;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface ILykkeTrader
    {
        Task PlaceOrdersAsync(ICompositeOrderBook orderBook);
    }
}
