using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Services.OrderBook
{
    public class BestPriceEvaluator : IBestPriceEvaluator
    {
        private readonly ILykkeExchange _lykkeExchange;
        private readonly ICompositeExchange _compositeExchange;
        private readonly IDictionary<string, decimal> _assetMinHalfSpread;
        private readonly IAssetPairDictionary _assetPairDictionary;

        public BestPriceEvaluator(ILykkeExchange lykkeExchange,
            ICompositeExchange compositeExchange,
            IAssetPairDictionary assetPairDictionary,
            IDictionary<string, decimal> assetMinHalfSpread)
        {
            _lykkeExchange = lykkeExchange;
            _compositeExchange = compositeExchange;
            _assetMinHalfSpread = assetMinHalfSpread;
            _assetPairDictionary = assetPairDictionary;
        }

        public decimal GetMinAsk(string assetPairId)
        {
            decimal bestAsk = GetBestAsk(assetPairId);
            decimal bestBid = GetBestBid(assetPairId);

            decimal middle = 0;
            if (bestAsk > bestBid)
            {
                middle = (bestAsk + bestBid) / 2;
            }
            else
            {
                middle = bestAsk;
            }

            return middle + GetMinHalfSpread(assetPairId);
        }

        public decimal GetMaxBid(string assetPairId)
        {
            decimal bestAsk = GetBestAsk(assetPairId);
            decimal bestBid = GetBestBid(assetPairId);

            decimal middle = 0;
            if (bestAsk > bestBid)
            {
                middle = (bestAsk + bestBid) / 2;
            }
            else
            {
                middle = bestBid;
            }

            return middle - GetMinHalfSpread(assetPairId);
        }

        private decimal GetBestAsk(string assetPairId)
        {
            var lykkeOrderBook = _lykkeExchange.GetOrderBook(assetPairId);
            _compositeExchange.TryGetValue(assetPairId, out var compositeOrderBook);

            decimal bestAsk = 0;
            if (lykkeOrderBook != null && lykkeOrderBook.Asks.Any())
            {
                bestAsk = lykkeOrderBook.Asks.Min(p=>p.Price);
            }
            else if (compositeOrderBook != null && compositeOrderBook.Asks.Any())
            {
                bestAsk = compositeOrderBook.Asks.Min(p => p.Price);
            }

            return bestAsk;
        }

        private decimal GetBestBid(string assetPairId)
        {
            var lykkeOrderBook = _lykkeExchange.GetOrderBook(assetPairId);
            _compositeExchange.TryGetValue(assetPairId, out var compositeOrderBook);

            decimal bestBid = 0;
            if (lykkeOrderBook != null && lykkeOrderBook.Bids.Any())
            {
                bestBid = lykkeOrderBook.Bids.Max(p=>p.Price);
            }
            else if (compositeOrderBook != null && compositeOrderBook.Bids.Any())
            {
                bestBid = compositeOrderBook.Bids.Max(p => p.Price);
            }

            return bestBid;
        }

        private decimal GetMinHalfSpread(string assetPairId)
        {
            return _assetMinHalfSpread[_assetPairDictionary[assetPairId].QuotingAssetId];
        }

    }
}
