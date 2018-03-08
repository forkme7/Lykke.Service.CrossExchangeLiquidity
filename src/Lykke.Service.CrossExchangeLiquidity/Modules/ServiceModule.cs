using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Services;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using Lykke.Service.CrossExchangeLiquidity.Services;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Services.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.CrossExchangeLiquidity.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<CrossExchangeLiquiditySettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<CrossExchangeLiquiditySettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            // TODO: Add your dependencies here
            RegisterLykkeExchange(builder);
            RegisterOrderBook(builder);

            builder.Populate(_services);
        }

        private void RegisterOrderBook(ContainerBuilder builder)
        {
            builder.RegisterInstance(new CompositeFilter(new[]
                    {new SourcesAssetPairIdsFilter(_settings.CurrentValue.OrderBook.Filter)}))
                .As<IOrderBookFilter>();

            builder.RegisterType<CompositeExchange>()
                .As<ICompositeExchange>();

            builder.RegisterType<OrderBookProcessor>()
                .As<IOrderBookProcessor>();

            builder.RegisterType<OrderBookDeserializer>()
                .As<IMessageDeserializer<OrderBook>>();

            builder.RegisterType<OrderBookSubscriber>()
                .As<IStartable>()
                .WithParameter("settings", _settings.CurrentValue.OrderBook.Source)
                .AutoActivate()
                .SingleInstance();
        }

        private void RegisterLykkeExchange(ContainerBuilder builder)
        {
            var socketLog = new SocketLogDynamic(i => { }, str => _log.WriteInfo("MatchingEngineClient", "", str));
            var tcpMatchingEngineClient = new TcpMatchingEngineClient(_settings.CurrentValue.LykkeExchange.IpEndpoint.GetClientIpEndPoint(), 
                socketLog);
            tcpMatchingEngineClient.Start();

            builder.RegisterType<MatchingEngineRetryAdapter>()
                .As<IMatchingEngineClient>()
                .WithParameter("settings", _settings.CurrentValue.LykkeExchange.Retry)
                .WithParameter("matchingEngineClient", tcpMatchingEngineClient);

            builder.RegisterInstance(new TopFilter(_settings.CurrentValue.LykkeExchange.Filter))
                .As<ITradeFilter>();

            builder.RegisterType<MatchingEngineTrader>()
                .As<ITrader>()
                .WithParameter("settings", _settings.CurrentValue.LykkeExchange);
        }
    }
}
