namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook
{
    public class SourcedVolumePrice
    {
        public decimal Price { get; }

        public decimal Volume { get; }

        public string Source { get; }

        public SourcedVolumePrice(VolumePrice volumePrice, string source) : 
            this(volumePrice.Price, volumePrice.Volume, source)
        {
        }

        public SourcedVolumePrice(decimal price, decimal volume, string source)
        {
            Price = price;
            Volume = volume;
            Source = source;
        }
    }
}
