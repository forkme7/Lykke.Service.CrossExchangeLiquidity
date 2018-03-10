using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.MatchingEngine.Connector.Abstractions.Models;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange.Helpers
{
    public class MultiLimitOrderModelEqualityComparer : IEqualityComparer<MultiLimitOrderModel>
    {
        private readonly MultiOrderItemModelEqualityComparer _multiOrderItemModelEqualityComparer = new MultiOrderItemModelEqualityComparer();

        public bool Equals(MultiLimitOrderModel x, MultiLimitOrderModel y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if(! string.Equals(x.AssetId,y.AssetId, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.Equals(x.ClientId, y.ClientId, StringComparison.OrdinalIgnoreCase))
                return false;

            foreach (MultiOrderItemModel multiOrderItemModel in x.Orders)
            {
                if (!y.Orders.Contains(multiOrderItemModel, _multiOrderItemModelEqualityComparer))
                    return false;
            }

            return x.Orders.Count == y.Orders.Count;
        }

        public int GetHashCode(MultiLimitOrderModel obj)
        {
            int hashCode = obj.AssetId.GetHashCode() ^ obj.ClientId.GetHashCode();

            foreach (MultiOrderItemModel multiOrderItemModel in obj.Orders)
            {
                hashCode ^= _multiOrderItemModelEqualityComparer.GetHashCode(multiOrderItemModel);
            }

            return hashCode;
        }
    }
}
