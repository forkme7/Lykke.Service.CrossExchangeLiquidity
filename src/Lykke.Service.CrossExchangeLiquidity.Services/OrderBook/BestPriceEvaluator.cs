using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Services.OrderBook
{
    public class BestPriceEvaluator: IBestPriceEvaluator
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
            var compositeOrderBook = _compositeExchange[assetPairId];

            decimal bestAsk = 0;
            if (lykkeOrderBook.Asks.Any())
            {
                bestAsk = lykkeOrderBook.Asks.First().Price;
            }
            else if (compositeOrderBook.Asks.Any())
            {
                bestAsk = compositeOrderBook.Asks.First().Price;
            }

            return bestAsk;
        }

        private decimal GetBestBid(string assetPairId)
        {
            var lykkeOrderBook = _lykkeExchange.GetOrderBook(assetPairId);
            var compositeOrderBook = _compositeExchange[assetPairId];

            decimal bestBid = 0;
            if (lykkeOrderBook.Bids.Any())
            {
                bestBid = lykkeOrderBook.Bids.First().Price;
            }
            else if (compositeOrderBook.Bids.Any())
            {
                bestBid = compositeOrderBook.Bids.First().Price;
            }

            return bestBid;
        }

        private decimal GetMinHalfSpread(string assetPairId)
        {
            return _assetMinHalfSpread[_assetPairDictionary[assetPairId].BaseAssetId];
        }

    }
}
