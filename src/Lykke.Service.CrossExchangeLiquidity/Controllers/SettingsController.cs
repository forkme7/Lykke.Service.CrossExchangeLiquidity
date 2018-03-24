using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.CrossExchangeLiquidity.Settings;
using Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.CrossExchangeLiquidity.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        private readonly CrossExchangeLiquiditySettings _settings;

        public SettingsController(CrossExchangeLiquiditySettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Returns service settings.
        /// </summary>
        /// <returns>Service settings.</returns>
        [HttpGet]
        [SwaggerOperation("Settings")]
        [Produces("application/json", Type = typeof(CrossExchangeLiquiditySettings))]
        public IActionResult Get()
        {
            return Ok(_settings);
        }
    }
}
