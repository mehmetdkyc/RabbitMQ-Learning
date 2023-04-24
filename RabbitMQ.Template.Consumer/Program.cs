
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var connectionString = "amqps://tnywxfoe:2zNoaSpp5PH7xkHY-v6o9GqIa6yPPM-E@moose.rmq.cloudamqp.com/tnywxfoe";

//Bağlantıyı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new(connectionString);

//Bağlantıyı Aktifleştirme ve Kanal Açma
using IConnection connection = factory.CreateConnection(); //bağlantıyı aktifleştirdik.
using IModel channel = connection.CreateModel(); // kanalı açtık.

#region P2P(Point to Point) Tasarımı - Direct Exchange de diyebiliriz aynısı

string queuename = "example-p2p-queue";
channel.QueueDeclare(
    queue: queuename,
    durable: false,
    exclusive: false,
    autoDelete: false
    );
EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue:queuename,
    autoAck:false,
    consumer:consumer
    );

consumer.Received += (sender, e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
};

#endregion
#region Publish/Subscribe (Pub/Sub) Tasarımı
#endregion
#region Work Queue (İş Kuyruğu) Tasarımı
#endregion
#region Request/Response Tasarımı
#endregion
Console.Read();