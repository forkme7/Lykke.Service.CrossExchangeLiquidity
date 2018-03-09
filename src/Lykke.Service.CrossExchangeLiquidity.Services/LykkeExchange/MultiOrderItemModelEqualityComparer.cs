using System;
using System.Collections.Generic;
using System.Text;
using Lykke.MatchingEngine.Connector.Abstractions.Models;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange
{
    public class MultiOrderItemModelEqualityComparer : IEqualityComparer<MultiOrderItemModel>
    {
        public bool Equals(MultiOrderItemModel x, MultiOrderItemModel y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            return x.OrderAction == y.OrderAction
                   && x.Price.Equals(y.Price)
                   && x.Volume.Equals(y.Volume);
        }

        public int GetHashCode(MultiOrderItemModel obj)
        {
            return obj.Price.GetHashCode() ^ obj.Volume.GetHashCode() ^ obj.OrderAction.GetHashCode();
        }
    }
}
