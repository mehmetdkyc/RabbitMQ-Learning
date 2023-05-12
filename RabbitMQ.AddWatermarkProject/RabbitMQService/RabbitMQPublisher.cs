using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.AddWatermarkProject.RabbitMQService
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitmqClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitmqClientService)
        {
            _rabbitmqClientService = rabbitmqClientService;
        }

        public void Publish(string imageUrl)
        {
            var channel = _rabbitmqClientService.Connect();
            //ilgili kanala bağlanıp kuyruğumuza ve exchangeimize url'in pathini yolluyoruz.
            var bodyString = JsonSerializer.Serialize(imageUrl);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange:RabbitMQClientService.ExchangeName,RabbitMQClientService.RoutingWatermark,properties,bodyByte);
        }
    }
}
