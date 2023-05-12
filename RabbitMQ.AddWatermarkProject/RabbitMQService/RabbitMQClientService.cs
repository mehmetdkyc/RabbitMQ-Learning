using RabbitMQ.Client;

namespace RabbitMQ.AddWatermarkProject.RabbitMQService
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string Queuename = "watermark-queue-image";
        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory,ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            
            if (_channel != null && _channel.IsOpen)
            {
                return _channel;
            }
            _channel = _connection.CreateModel();
            //watermark eklemek için ilgili exchange ve queueyu oluşturup bind etme.
            _channel.ExchangeDeclare(ExchangeName,type:"direct",true,false);
            _channel.QueueDeclare(Queuename,true,false,false,null);
            _channel.QueueBind(Queuename,ExchangeName,RoutingWatermark,null);
            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu...");
            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ ile bağlantı koptu...");
        }
    }
}
