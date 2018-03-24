namespace Lykke.Service.CrossExchangeLiquidity.Shared.Models.Orders
{
    public class LykkeLimitOrderFeeModel
    {
        public int Type { get; set; }

        public double MakerSize { get; set; }

        public double TakerSize { get; set; }

        public string SourceClientId { get; set; }

        public string TargetClientId { get; set; }

        public int MakerSizeType { get; set; }

        public int TakerSizeType { get; set; }

        public string[] AssetId { get; set; }

        public double MakerFeeModificator { get; set; }
    }
}
