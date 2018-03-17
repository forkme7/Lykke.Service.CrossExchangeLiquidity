using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings.ExternalExchange
{
    public interface IExternalBalanceServicesSettings
    {
        ExternalBalanceServiceSettings[] Balances { get; set; }
    }
}
