using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook
{
    public class SourcedVolumePrice : VolumePrice
    {
        public string Source { get; }

        public SourcedVolumePrice(ExternalVolumePrice volumePrice, string source) :
            this(volumePrice.Price, volumePrice.Volume, source)
        {
        }

        public SourcedVolumePrice(decimal price, decimal volume, string source) : base(price, volume)
        {
            Source = source;
        }
    }
}
