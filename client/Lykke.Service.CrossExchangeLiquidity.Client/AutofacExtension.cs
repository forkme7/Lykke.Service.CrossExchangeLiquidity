using System;
using Autofac;
using Common.Log;

namespace Lykke.Service.CrossExchangeLiquidity.Client
{
    public static class AutofacExtension
    {
        public static void RegisterCrossExchangeLiquidityClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<CrossExchangeLiquidityClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<ICrossExchangeLiquidityClient>()
                .SingleInstance();
        }

        public static void RegisterCrossExchangeLiquidityClient(this ContainerBuilder builder, CrossExchangeLiquidityServiceClientSettings settings, ILog log)
        {
            builder.RegisterCrossExchangeLiquidityClient(settings?.ServiceUrl, log);
        }
    }
}
