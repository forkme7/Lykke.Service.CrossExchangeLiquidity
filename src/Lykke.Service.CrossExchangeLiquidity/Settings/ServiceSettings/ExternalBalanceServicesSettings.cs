using Lykke.Service.CrossExchangeLiquidity.Core.Settings.ExternalExchange;
using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class ExternalBalanceServicesSettings : IExternalBalanceServicesSettings
    {
        public ExternalBalanceServiceSettings[] Balances { get; set; }
    }
}
