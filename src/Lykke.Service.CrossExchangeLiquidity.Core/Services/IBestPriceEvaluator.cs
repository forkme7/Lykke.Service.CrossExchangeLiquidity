using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface IBestPriceEvaluator
    {
        decimal GetMinAsk(string assetPairId);
        decimal GetMaxBid(string assetPairId);
    }
}
