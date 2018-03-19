using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using Lykke.Service.CrossExchangeLiquidity.Services.OrderBook;

namespace Lykke.Service.CrossExchangeLiquidity.Services.RabbitMQ
{
    public class Subscriber<T> : IStartable, IStopable
    {
        private readonly IMessageProcessor<T> _processor;
        private readonly ILog _log;
        private readonly IMessageDeserializer<T> _deserializer;
        private readonly IRabbitMqSettings _settings;
        private RabbitMqSubscriber<T> _subscriber;
        private readonly bool _durable = true;

        public Subscriber(
            ILog log,
            IMessageProcessor<T> processor,
            IMessageDeserializer<T> deserializer,
            IShutdownManager shutdownManager,
            IRabbitMqSettings settings,
            bool durable = true)
        {
            _log = log;
            _processor = processor;
            _deserializer = deserializer;
            _settings = settings;
            shutdownManager.Register(this);
            _durable = durable;
        }

        public void Start()
        {
            _log.WriteInfo(GetType().Name, MethodBase.GetCurrentMethod().Name, ">>");

            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString,
                    _settings.ExchangeName,
                    _settings.NameOfEndpoint);

            if (_durable)
            {
                settings.MakeDurable();
            }

            _subscriber = new RabbitMqSubscriber<T>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(_deserializer)
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();

            _log.WriteInfo(GetType().Name, MethodBase.GetCurrentMethod().Name, $"<< RabbitMq subscriber {typeof(T).Name} is starting.");
        }

        private async Task ProcessMessageAsync(T model)
        {
            try
            {
                await _processor.ProcessAsync(model);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(GetType().Name, nameof(ProcessMessageAsync), ex);
                throw;
            }
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _log.WriteInfo(GetType().Name, MethodBase.GetCurrentMethod().Name, ">>");

            _subscriber?.Stop();

            _log.WriteInfo(GetType().Name, MethodBase.GetCurrentMethod().Name, "<< RabbitMq subscriber is stopping.");
        }

    }
}
