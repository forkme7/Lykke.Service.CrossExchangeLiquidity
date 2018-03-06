using Common.Log;
using Domain = Lykke.Service.CrossExchangeLiquidity.Core.Domain.OrderBook;
using Lykke.RabbitMqBroker.Subscriber;
using Newtonsoft.Json;

namespace Lykke.Service.CrossExchangeLiquidity.Services.OrderBook
{
    public class OrderBookDeserializer : IMessageDeserializer<Domain.OrderBook>
    {
        private readonly ILog _log;

        public OrderBookDeserializer(ILog log)
        {
            _log = log;
        }

        public Domain.OrderBook Deserialize(byte[] data)
        {
            var dataStr = System.Text.Encoding.Default.GetString(data);
            try
            {
                return JsonConvert.DeserializeObject<Domain.OrderBook>(dataStr);
            }
            catch (JsonSerializationException ex)
            {
                _log.WriteErrorAsync(nameof(OrderBookDeserializer), nameof(Deserialize), ex);
            }

            return null;
        }
    }
}
