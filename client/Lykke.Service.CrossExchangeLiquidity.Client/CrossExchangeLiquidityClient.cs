using System;
using Common.Log;

namespace Lykke.Service.CrossExchangeLiquidity.Client
{
    public class CrossExchangeLiquidityClient : ICrossExchangeLiquidityClient, IDisposable
    {
        private readonly ILog _log;

        public CrossExchangeLiquidityClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
