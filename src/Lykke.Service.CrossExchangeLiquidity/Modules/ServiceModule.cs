using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using AzureStorage.Tables;
using Common.Log;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.MatchingEngine.Connector.Services;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.Client;
using Lykke.Service.CrossExchangeLiquidity.AzureRepositories.AssetBalance;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalBalance;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeTrade;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.TradePart;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.VolumePrice;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.ExternalExchange;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Models.Balance;
using Lykke.Service.CrossExchangeLiquidity.Models.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Services;
using Lykke.Service.CrossExchangeLiquidity.Services.ExternalExchange;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Services.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Services.RabbitMQ;
using Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

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
            RegisterLykkeOrderBook(builder);
            RegisterUI(builder);

            builder.Populate(_services);
        }

        private void RegisterUI(ContainerBuilder builder)
        {
            var mapperProvider = new MapperProvider();
            IMapper mapper = mapperProvider.GetMapper();
            builder.RegisterInstance(mapper).As<IMapper>();

            builder.RegisterType<OrderBookProvider>()
                .As<IOrderBookProvider>();

            builder.RegisterType<VolumePriceFilterModelFactory>()
                .As<IVolumePriceFilterModelFactory>();

            builder.RegisterType<BalanceProvider>()
                .As<IBalanceProvider>();

            builder.RegisterInstance(_settings.CurrentValue)
                .As<CrossExchangeLiquiditySettings>();
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
                .As<IStartable>()
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
                .As<IStartable>()
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
            //Attention! Keep filters order!
            builder.Register(c => new IVolumePriceFilter[]
                {
                    new VolumePartVolumePriceFilter(_settings.CurrentValue.VolumePriceFilters.Assets.ToDictionary(a=>a.AssetId, a=>a.UseVolumePart),
                        c.Resolve<IAssetPairDictionary>()),
                    new MinVolumeVolumePriceFilter(_settings.CurrentValue.VolumePriceFilters.Assets.ToDictionary(a=>a.AssetId, a=>a.MinVolume),
                        c.Resolve<IAssetPairDictionary>()),
                    new ExternalBalanceVolumePriceFilter(c.Resolve<IExternalBalanceService>(),
                        c.Resolve<IAssetPairDictionary>()),
                    new RiskMarkupVolumePriceFilter(_settings.CurrentValue.VolumePriceFilters.Assets.ToDictionary(a=>a.AssetId, a=>a.RiskMarkup),
                        c.Resolve<IAssetPairDictionary>()),
                    new LykkeBalanceVolumePriceFilter(c.Resolve<ILykkeBalanceService>(),
                        c.Resolve<IAssetPairDictionary>()),
                    new TopVolumePriceFilter(_settings.CurrentValue.VolumePriceFilters.Count),
                    new BestPriceFilter(c.Resolve<IBestPriceEvaluator>())
                })
                .As<IEnumerable<IVolumePriceFilter>>()
                .SingleInstance();

            builder.RegisterType<CompositeVolumePriceFilter>()
                .As<IVolumePriceFilter>()
                .SingleInstance();

            builder.RegisterInstance(_settings.CurrentValue.MatchingEngineTrader)
                .As<IMatchingEngineTraderSettings>();

            builder.RegisterType<MatchingEngineTrader>()
                .As<ILykkeTrader>();
        }

        private void RegisterOrderBook(ContainerBuilder builder)
        {
            builder.RegisterInstance(new CompositeAnyExternalOrderBookFilter(
                    _settings.CurrentValue.ExternalOrderBook.Filter.Select(p => new SourceAssetPairIdsExternalOrderBookFilter(p))
                ))
                .As<IExternalOrderBookFilter>();

            builder.RegisterType<CompositeExchange>()
                .As<ICompositeExchange>()
                .SingleInstance();

            builder.RegisterType<ExternalOrderBookProcessor>()
                .As<IMessageProcessor<ExternalOrderBook>>();

            builder.RegisterType<Deserializer<ExternalOrderBook>>()
                .As<IMessageDeserializer<ExternalOrderBook>>();

            builder.RegisterType<Subscriber<ExternalOrderBook>>()
                .As<IStartable>()
                .WithParameter("settings", _settings.CurrentValue.ExternalOrderBook.Source)
                //.WithParameter("durable", false)
                .AutoActivate()
                .SingleInstance();
        }

        private void RegisterLykkeOrderBook(ContainerBuilder builder)
        {
            builder.RegisterType<BestPriceEvaluator>()
                .As<IBestPriceEvaluator>()
                .WithParameter("assetMinHalfSpread", _settings.CurrentValue.VolumePriceFilters.Assets.ToDictionary(a => a.AssetId, a => a.MinHalfSpread))
                .SingleInstance();

            builder.RegisterType<LykkeExchange>()
                .As<ILykkeExchange>()
                .WithParameter("settings", _settings.CurrentValue.MatchingEngineTrader)
                .SingleInstance();

            builder.RegisterInstance(new AssetPairIdsLykkeOrderBookFilter(_settings.CurrentValue.LykkeOrderBook.Filter))
                .As<ILykkeOrderBookFilter>();

            builder.RegisterType<LykkeOrderBookProcessor>()
                .As<IMessageProcessor<LykkeOrderBook>>();

            builder.RegisterType<Deserializer<LykkeOrderBook>>()
                .As<IMessageDeserializer<LykkeOrderBook>>();

            builder.RegisterType<Subscriber<LykkeOrderBook>>()
                .As<IStartable>()
                .WithParameter("settings", _settings.CurrentValue.LykkeOrderBook.Source)
                //.WithParameter("durable", false)
                .AutoActivate()
                .SingleInstance();
        }
    }
}
