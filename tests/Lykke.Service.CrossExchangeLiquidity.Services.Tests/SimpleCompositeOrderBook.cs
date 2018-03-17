using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests
{
    public class SimpleCompositeOrderBook : ICompositeOrderBook
    {
        public string AssetPairId { get; set; }

        public IEnumerable<SourcedVolumePrice> Asks { get; set; }

        public IEnumerable<SourcedVolumePrice> Bids { get; set; }

        public void AddOrUpdateOrderBook(IExternalOrderBook orderBook)
        {
            throw new NotImplementedException();
        }
    }
}
