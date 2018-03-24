using System.Collections.ObjectModel;

namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook.Filters
{
    public class LykkeBalanceVolumePriceFilterModel : VolumePriceFilterModel
    {
        public ReadOnlyDictionary<string, decimal> LykkeBalance { get; set; }
    }
}
