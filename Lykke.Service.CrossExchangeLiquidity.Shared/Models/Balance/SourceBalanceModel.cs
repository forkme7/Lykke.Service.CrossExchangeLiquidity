using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.Balance
{
    public class SourceBalanceModel
    {
        public string Source { get; set; }

        public IEnumerable<BalanceModel> Balances { get; set; }
    }
}
