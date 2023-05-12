using Microsoft.Extensions.Hosting;
using RabbitMQ.AddWatermarkProject.RabbitMQService;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.AddWatermarkProject.BackgroundService
{
    public class ImageWatermarkProcessBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ILogger<ImageWatermarkProcessBackgroundService> _logger;
        private IModel _channel;
        public ImageWatermarkProcessBackgroundService(RabbitMQClientService rabbitMQClientService, ILogger<ImageWatermarkProcessBackgroundService> logger)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0,1,false); //ilgili kuyruklara birer birer yollamak demek burası

            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.Queuename,false,consumer); // ilgili kuyrağa gelen mesajları dinleme

            consumer.Received += Consumer_Received;
            return Task.CompletedTask;
        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            //resime watermark işleme yeri
          

            try
            {
                var imageName = JsonSerializer.Deserialize<string>(Encoding.UTF8.GetString(@event.Body.ToArray())); //image'ın ismini burda elde ettik.

                var path = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\images", imageName);
                var siteName = "www.mehmetdokuyucu.com";
                //resime ekleyeceğimiz yazıyı ayarlama ve ekleme
                using var img = Image.FromFile(path);

                Graphics g = Graphics.FromImage(img);
                using (Font myfont = new Font("Arial", 36))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(siteName, myfont, Brushes.White, new Rectangle(0, 0, img.Width, img.Height), format);
                }


                var watermarkPath = Path.Combine(Directory.GetCurrentDirectory() + "\\wwwroot\\images\\watermarks\\");
                img.Save(watermarkPath + imageName);
                img.Dispose();
                //graphic.Dispose();

                _channel.BasicAck(@event.DeliveryTag,false); //bu aşamaya geldiyse demek başarıyla mesajı işlemiştir o yüzden de silebilirsin haberini iletiyor.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            return Task.CompletedTask;
        }
    }
}
