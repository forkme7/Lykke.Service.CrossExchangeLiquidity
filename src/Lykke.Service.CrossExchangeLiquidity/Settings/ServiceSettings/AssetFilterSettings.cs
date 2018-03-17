using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class AssetFilterSettings
    {
        public string AssetId { get; set; }
        public decimal RiskMarkup { get; set; }
        public decimal UseVolumePart { get; set; }
        public decimal MinVolume { get; set; }
        public decimal MinHalfSpread { get; set; }
    }
}
