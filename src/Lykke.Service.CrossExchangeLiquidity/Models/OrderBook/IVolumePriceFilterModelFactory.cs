using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook.Filters;

namespace Lykke.Service.CrossExchangeLiquidity.Models.OrderBook
{
    public interface IVolumePriceFilterModelFactory
    {
        VolumePriceFilterModel GetModel(IVolumePriceFilter filter, string assetPairId);
    }
}
