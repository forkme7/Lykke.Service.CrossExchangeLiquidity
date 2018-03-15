using System.Collections.ObjectModel;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.ExternalExchange;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Settings.ExternalExchange
{
    public class ExternalBalanceServiceSettings
    {
        public string Source { get; set; }
        public AssetValueSettings[] AssetValues { get; set; }
    }
}
