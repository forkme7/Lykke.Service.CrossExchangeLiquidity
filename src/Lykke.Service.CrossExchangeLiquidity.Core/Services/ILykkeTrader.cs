using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Models;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface ILykkeTrader
    {
        MultiLimitOrderModel GetLastModel(string assetPairId);

        MultiLimitOrderModel[] GetLastModels();

        Task PlaceOrdersAsync(ICompositeOrderBook orderBook);
    }
}
