using System;
using System.Collections.Generic;
using System.Text;
using Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.CrossExchangeLiquidity.Services.OrderBook;
using Newtonsoft.Json;

namespace Lykke.Service.CrossExchangeLiquidity.Services.RabbitMQ
{
    public class Deserializer<T> : IMessageDeserializer<T>
    {
        private readonly ILog _log;

        public Deserializer(ILog log)
        {
            _log = log;
        }

        public T Deserialize(byte[] data)
        {
            var dataStr = System.Text.Encoding.Default.GetString(data);
            try
            {
                return JsonConvert.DeserializeObject<T>(dataStr);
            }
            catch (JsonSerializationException ex)
            {
                _log.WriteErrorAsync(GetType().Name, nameof(Deserialize), ex);
            }

            return default(T);
        }
    }
}
