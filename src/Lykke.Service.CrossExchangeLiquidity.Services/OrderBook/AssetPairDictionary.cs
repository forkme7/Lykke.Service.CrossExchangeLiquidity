using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Services.OrderBook
{
    public class AssetPairDictionary : ReadOnlyDictionary<string, AssetPair>, IAssetPairDictionary
    {
        public AssetPairDictionary(IAssetsService assetsService) : base(assetsService.AssetPairGetAll()
            .ToDictionary(p => p.Id))
        {
        }
    }
}
