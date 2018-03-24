using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Models.Balance;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.Balance;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lykke.Service.CrossExchangeLiquidity.Controllers
{
    [Route("api/[controller]/[action]")]
    public class BalanceController : Controller
    {
        private readonly ILykkeBalanceService _lykkeBalanceService;
        private readonly IExternalBalanceService _externalBalanceService;
        private readonly IBalanceProvider _balanceProvider;

        public BalanceController(ILykkeBalanceService lykkeBalanceService,
            IExternalBalanceService externalBalanceService,
            IBalanceProvider balanceProvider)
        {
            _lykkeBalanceService = lykkeBalanceService;
            _externalBalanceService = externalBalanceService;
            _balanceProvider = balanceProvider;
        }

        /// <summary>
        /// Returns asset's balance from Lykke exchange.
        /// </summary>
        /// <param name="assetId">Identifier of asset. Returns all if this param is skipped.</param>
        /// <returns>Asset's balance.</returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<BalanceModel>))]
        public IActionResult Lykke(string assetId = null)
        {
            IEnumerable<BalanceModel> model;

            if (string.IsNullOrEmpty(assetId))
            {
                model = _balanceProvider.Get(_lykkeBalanceService.GetBalances());
            }
            else
            {
                model = _balanceProvider.Get(assetId, _lykkeBalanceService.GetAssetBalance(assetId));
            }

            return Ok(model);
        }

        /// <summary>
        /// Returns asset's balance from external exchanges.
        /// </summary>
        /// <param name="source">Identifier of exchange. Returns all if this param is skipped.</param>
        /// <param name="assetId">Identifier of asset. Returns all if this param is skipped.</param>
        /// <returns>Asset's balance.</returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<SourceBalanceModel>))]
        public IActionResult External(string source=null, string assetId=null)
        {
            IEnumerable<SourceBalanceModel> model;
            if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(assetId))
            {
                model = _balanceProvider.Get(source, assetId,
                    _externalBalanceService.GetAssetBalance(source, assetId));
            }
            else
            {
                ReadOnlyDictionary<string, ReadOnlyDictionary<string, decimal>> balances =
                    _externalBalanceService.GetBalances();
                if (!string.IsNullOrEmpty(source))
                {
                    model = _balanceProvider.Get(source, balances);
                }
                else if(!string.IsNullOrEmpty(assetId))
                {
                    model = _balanceProvider.GetByAsset(assetId, balances);
                }
                else
                {
                    model = _balanceProvider.Get(balances);
                }
            }

            return Ok(model);
        }
    }
}
