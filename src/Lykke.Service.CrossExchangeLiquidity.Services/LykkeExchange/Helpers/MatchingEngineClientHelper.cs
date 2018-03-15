using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange.Helpers
{
    internal class MatchingEngineClientHelper
    {
        public string CreateRequestId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
