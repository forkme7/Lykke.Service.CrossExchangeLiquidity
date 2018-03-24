using System.Collections.ObjectModel;

namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook.Filters
{
    public class ExternalBalanceVolumePriceFilterModel : VolumePriceFilterModel
    {
        public ReadOnlyDictionary<string, ReadOnlyDictionary<string, decimal>> ExternalBalance { get; set; }
    }
}
