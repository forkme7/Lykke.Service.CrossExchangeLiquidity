using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using Domain = Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Services.OrderBook
{
    public class OrderBookSubscriber : IStartable, IStopable
    {
        private readonly ILog _log;
        private readonly IOrderBookProcessor _processor;
        private readonly IMessageDeserializer<Domain.OrderBook> _deserializer;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private RabbitMqSubscriber<Domain.OrderBook> _subscriber;

        public OrderBookSubscriber(
            ILog log,
            IOrderBookProcessor processor,
            IMessageDeserializer<Domain.OrderBook> deserializer,
            IShutdownManager shutdownManager,
            RabbitMqSettings rabbitMqSettings)
        {
            _log = log;
            _processor = processor;
            _deserializer = deserializer;
            _rabbitMqSettings = rabbitMqSettings;
            shutdownManager.Register(this);
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_rabbitMqSettings.ConnectionString,
                    _rabbitMqSettings.ExchangeName,
                    _rabbitMqSettings.NameOfEndpoint)
                .MakeDurable();

            _subscriber = new RabbitMqSubscriber<Domain.OrderBook>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(_deserializer)
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        private async Task ProcessMessageAsync(Domain.OrderBook orderBook)
        {
            try
            {
                _processor.Process(orderBook);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(OrderBookSubscriber), nameof(ProcessMessageAsync), ex);
                throw;
            }
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }
    }
}
