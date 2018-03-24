using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.Orders
{
    public class LykkeMultiLimitOrderModel
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public bool CancelPreviousOrders { get; set; }
        public IReadOnlyList<LykkeMultiOrderItemModel> Orders { get; set; }
    }
}
