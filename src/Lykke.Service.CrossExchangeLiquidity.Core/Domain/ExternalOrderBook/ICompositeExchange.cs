using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook
{
    public interface ICompositeExchange : IReadOnlyDictionary<string, ICompositeOrderBook>
    {
        void AddOrUpdateOrderBook(IExternalOrderBook orderBook);
    }
}
