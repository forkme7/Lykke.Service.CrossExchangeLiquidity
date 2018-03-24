using AutoMapper;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.CrossExchangeLiquidity.Controllers
{
    [Route("api/[controller]/[action]")]
    public class OrdersController : Controller
    {
        private readonly ILykkeTrader _lykkeTrader;
        private readonly IMapper _mapper;

        public OrdersController(ILykkeTrader lykkeTrader, IMapper mapper)
        {
            _lykkeTrader = lykkeTrader;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns orders currently placed on Lykke exchange.
        /// </summary>
        /// <param name="assetPairId">Identifier of asset pair. Returns all if this param is skipped.</param>
        /// <returns>Orders</returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(LykkeMultiLimitOrderModel[]))]
        public IActionResult Lykke(string assetPairId = null)
        {
            MultiLimitOrderModel[] orders;
            if (string.IsNullOrEmpty(assetPairId))
            {
                orders = _lykkeTrader.GetLastModels();
            }
            else
            {
                orders = new[] {_lykkeTrader.GetLastModel(assetPairId)};
            }

            var models = _mapper.Map<LykkeMultiLimitOrderModel[]>(orders);
            return Ok(models);
        }
    }
}
