using Lykke.Service.Assets.Client.Models;
using System.Collections.Generic;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface IAssetPairDictionary : IReadOnlyDictionary<string, AssetPair>
    {
    }
}
