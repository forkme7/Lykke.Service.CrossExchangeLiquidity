using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class VolumePriceFilterSettings
    {
        public int Count { get; set; }
        public IReadOnlyCollection<AssetFilterSettings> Assets { get; set; }
    }
}
