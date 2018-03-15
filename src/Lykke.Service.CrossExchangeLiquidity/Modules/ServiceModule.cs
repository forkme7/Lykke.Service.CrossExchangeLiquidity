using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Services;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Client;
using Lykke.Service.CrossExchangeLiquidity.AzureRepositories.AssetBalance;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.TradePart;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.ExternalExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Services;
using Lykke.Service.CrossExchangeLiquidity.Services.ExternalExchange;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Services.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Services.RabbitMQ;
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
            RegisterExternalBalanceService(builder);
            RegisterAssetPairDictionary(builder);
            RegisterLykkeBalanceService(builder);
            RegisterLimitOrderMessageSubscriber(builder);
            RegisterMatchingEngine(builder);
            RegisterOrderBook(builder);

            builder.Populate(_services);
        }

        private void RegisterExternalBalanceService(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings.CurrentValue.ExternalBalance)
                .As<IExternalBalanceServicesSettings>();

            builder.RegisterInstance<IAssetBalanceRepository>(new AssetBalanceRepository(
                AzureTableStorage<AssetBalanceEntity>.Create(_settings.ConnectionString(x => x.Db.EntitiesConnString),
                    _settings.CurrentValue.Db.AssetBalanceTableName, _log)));

            builder.RegisterType<ExternalBalanceService>()
                .As<IExternalBalanceService>()
                .AutoActivate()
                .SingleInstance();
        }
        private void RegisterAssetPairDictionary(ContainerBuilder builder)
        {
            builder.RegisterInstance(new AssetsService(new Uri(_settings.CurrentValue.AssetsServiceUrl)))
                .As<IAssetsService>()
                .SingleInstance();

            builder.RegisterType<AssetPairDictionary>()
                .As<IAssetPairDictionary>()
                .SingleInstance();
        }

        private void RegisterLykkeBalanceService(ContainerBuilder builder)
        {
            builder.RegisterBalancesClient(_settings.CurrentValue.BalancesServiceUrl, _log);

            builder.RegisterInstance(_settings.CurrentValue.LykkeBalance)
                .As<ILykkeBalanceServiceSettings>();

            builder.RegisterType<LykkeBalanceService>()
                .As<ILykkeBalanceService>()
                .AutoActivate()
                .SingleInstance();
        }

        private void RegisterLimitOrderMessageSubscriber(ContainerBuilder builder)
        {            
            builder.RegisterType<ClientIdTradePartFilter>()
                .As<ITradePartFilter>()
                .WithParameter("settings", _settings.CurrentValue.MatchingEngineTrader);

            builder.RegisterType<LimitOrderMessageProcessor>()
                .As<IMessageProcessor<LimitOrderMessage>>();            

            builder.RegisterType<Deserializer<LimitOrderMessage>>()
                .As<IMessageDeserializer<LimitOrderMessage>>();

            builder.RegisterType<Subscriber<LimitOrderMessage>>()
                .As<IStartable>()
                .WithParameter("settings", _settings.CurrentValue.MatchingEngineTrader.LimitOrders)
                .AutoActivate()
                .SingleInstance();
        }

        private void RegisterMatchingEngine(ContainerBuilder builder)
        {
            //MatchingEngineRetryAdapter
            var socketLog = new SocketLogDynamic(i => { }, str => _log.WriteInfo("MatchingEngineClient", "", str));
            var tcpMatchingEngineClient = new TcpMatchingEngineClient(_settings.CurrentValue.MatchingEngineTrader.IpEndpoint.GetClientIpEndPoint(),
                socketLog);
            tcpMatchingEngineClient.Start();

            builder.RegisterInstance(_settings.CurrentValue.MatchingEngineTrader.Retry)
                .As<IMatchingEngineRetryAdapterSettings>();

            builder.RegisterType<MatchingEngineRetryAdapter>()
                .As<IMatchingEngineClient>()
                .WithParameter("matchingEngineClient", tcpMatchingEngineClient);

            //MatchingEngineTrader
            builder.RegisterType<ExternalBalanceVolumePriceFilter>();
            builder.RegisterType<LykkeBalanceVolumePriceFilter>();
            builder.Register(c => new CompositeVolumePriceFilter(new IVolumePriceFilter[]
                {
                    c.Resolve<ExternalBalanceVolumePriceFilter>(),
                    c.Resolve<LykkeBalanceVolumePriceFilter>(),
                    new TopVolumePriceFilter(_settings.CurrentValue.MatchingEngineTrader.Filter)
                }))
                .As<IVolumePriceFilter>();

            builder.RegisterInstance(_settings.CurrentValue.MatchingEngineTrader)
                .As<IMatchingEngineTraderSettings>();

            builder.RegisterType<MatchingEngineTrader>()
                .As<ITrader>();
        }

        private void RegisterOrderBook(ContainerBuilder builder)
        {
            builder.RegisterInstance(new CompositeOrderBookFilter(new[]
                    {new SourcesAssetPairIdsOrderBookFilter(_settings.CurrentValue.OrderBook.Filter)}))
                .As<IOrderBookFilter>();

            builder.RegisterType<CompositeExchange>()
                .As<ICompositeExchange>();

            builder.RegisterType<OrderBookProcessor>()
                .As<IMessageProcessor<OrderBook>>();

            builder.RegisterType<Deserializer<OrderBook>>()
                .As<IMessageDeserializer<OrderBook>>();

            builder.RegisterType<Subscriber<OrderBook>>()
                .As<IStartable>()
                .WithParameter("settings", _settings.CurrentValue.OrderBook.Source)
                .AutoActivate()
                .SingleInstance();
        }
    }
}
