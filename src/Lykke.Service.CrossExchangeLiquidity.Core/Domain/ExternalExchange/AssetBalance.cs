using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalExchange
{
    public class AssetBalance: IAssetBalance
    {        
        public string Source { get; private set; }
        public string AssetId { get; private set; }
        public decimal Value { get; set; }

        public AssetBalance()
        {
        }

        public AssetBalance(string source, string assetId)
        {
            Source = source;
            AssetId = assetId;
        }
    }
}
