using AutoMapper;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Models.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Shared;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.CrossExchangeLiquidity.Controllers
{
    [Route("api/[controller]/[action]")]
    public class OrderBookController : Controller
    {
        private readonly ILykkeExchange _lykkeExchange;
        private readonly ICompositeExchange _compositeExchange;
        private readonly IOrderBookProvider _orderBookProvider;
        private readonly IEnumerable<IVolumePriceFilter> _volumePriceFilters;
        private readonly IVolumePriceFilterModelFactory _volumePriceFilterModelFactory;
        private readonly IMapper _mapper;

        public OrderBookController(ILykkeExchange lykkeExchange, 
            ICompositeExchange compositeExchange,
            IOrderBookProvider orderBookProvider,
            IMapper mapper,
            IEnumerable<IVolumePriceFilter> volumePriceFilters,
            IVolumePriceFilterModelFactory volumePriceFilterModelFactory)
        {
            _lykkeExchange = lykkeExchange;
            _compositeExchange = compositeExchange;
            _orderBookProvider = orderBookProvider;
            _mapper = mapper;
            _volumePriceFilters = volumePriceFilters;
            _volumePriceFilterModelFactory = volumePriceFilterModelFactory;
        }

        /// <summary>
        /// Returns orderbook from Lykke exchange.
        /// </summary>
        /// <param name="assetPairId">Identifier of asset pair. Returns all if this param is skipped.</param>
        /// <returns>Orderbook</returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<OrderBookModel>))]
        public IActionResult Lykke(string assetPairId = null)
        {
            IEnumerable<OrderBookModel> models;
            if (string.IsNullOrEmpty(assetPairId))
            {
                models = _orderBookProvider.Get(Constants.LykkeSource, _lykkeExchange.GetOrderBooks());
            }
            else
            {
                models = _orderBookProvider.Get(Constants.LykkeSource, _lykkeExchange.GetOrderBook(assetPairId));
            }

            return Ok(models);
        }

        /// <summary>
        /// Returns composite orderbook from external exchanges.
        /// </summary>
        /// <param name="assetPairId">Identifier of asset pair. Returns all if this param is skipped.</param>
        /// <returns>Orderbook</returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<CompositeOrderBookModel>))]
        public IActionResult ExternalComposite(string assetPairId = null)
        {
            IEnumerable<ICompositeOrderBook> compositeOrderBooks = null;
            if (string.IsNullOrEmpty(assetPairId))
            {
                compositeOrderBooks = _compositeExchange.Values.ToArray();
            }
            else
            {
                if (_compositeExchange.TryGetValue(assetPairId, out var compositeOrderBook))
                {
                    compositeOrderBooks = new[] {compositeOrderBook};
                }
            }

            var model = _mapper.Map<IEnumerable<CompositeOrderBookModel>>(compositeOrderBooks);
            return Ok(model);
        }

        /// <summary>
        /// Returns orderbook from external exchange.
        /// </summary>
        /// <param name="source">Identifier of external exchange. Returns all if this param is skipped.</param>
        /// <param name="assetPairId">Identifier of asset pair. Returns all if this param is skipped.</param>
        /// <returns>Orderbook</returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<OrderBookModel>))]
        public IActionResult External(string source, string assetPairId)
        {
            IEnumerable<OrderBookModel> model = null;

            if (!string.IsNullOrEmpty(assetPairId))
            {
                if (_compositeExchange.TryGetValue(assetPairId, out var compositeOrderBook))
                {
                    if (string.IsNullOrEmpty(source))
                    {
                        model = _orderBookProvider.Get(compositeOrderBook);
                    }
                    else
                    {
                        model = _orderBookProvider.Get(source, compositeOrderBook);
                    }
                }
            }
            else if (string.IsNullOrEmpty(source))
            {
                model = _orderBookProvider.Get(_compositeExchange);                
            }
            else
            {
                model = _orderBookProvider.Get(source, _compositeExchange);
            }

            return Ok(model);
        }

        /// <summary>
        /// Returns result of filtering composite orderbook from external exchanges for every specified filter.
        /// </summary>
        /// <param name="assetPairId">Identifier of asset pair.</param>
        /// <returns>Orderbook</returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<FilteredCompositeOrderBookModel>))]
        public IActionResult ExternalCompositeFiltered(string assetPairId)
        {
            if (string.IsNullOrEmpty(assetPairId) || !_compositeExchange.ContainsKey(assetPairId))
            {
                return Ok(null);
            }

            var list = new List<FilteredCompositeOrderBookModel>();
            ICompositeOrderBook compositeOrderBook = _compositeExchange[assetPairId];
            IEnumerable<SourcedVolumePrice> asks = compositeOrderBook.Asks;
            IEnumerable<SourcedVolumePrice> bids = compositeOrderBook.Bids;
            foreach (IVolumePriceFilter filter in _volumePriceFilters)
            {
                asks = filter.GetAsks(assetPairId, asks);
                bids = filter.GetBids(assetPairId, bids);
                var model = new FilteredCompositeOrderBookModel
                {
                    Filter = _volumePriceFilterModelFactory.GetModel(filter, assetPairId),
                    AssetPairId = assetPairId,
                    Asks = _mapper.Map<IEnumerable<SourcedVolumePriceModel>>(asks),
                    Bids = _mapper.Map<IEnumerable<SourcedVolumePriceModel>>(bids)
                };
                list.Add(model);
            }

            return Ok(list.ToArray());
        }
    }
}
