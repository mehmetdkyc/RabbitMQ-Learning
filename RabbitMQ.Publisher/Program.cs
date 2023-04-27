

using RabbitMQ.Client;
using System.Text;

var connectionString = "amqps://tnywxfoe:2zNoaSpp5PH7xkHY-v6o9GqIa6yPPM-E@moose.rmq.cloudamqp.com/tnywxfoe";
var connectionStringDocker = "amqp://localhost:5672";
//Bağlantıyı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new(connectionStringDocker);

//Bağlantıyı Aktifleştirme ve Kanal Açma
using IConnection connection = factory.CreateConnection(); //bağlantıyı aktifleştirdik.
using IModel channel = connection.CreateModel(); // kanalı açtık.

IBasicProperties properties = channel.CreateBasicProperties();
properties.Persistent = true; //mesajları kalıcı hale getirme
/// <summary>
/// Yukarısı fix aynıdır genelde.
/// </summary>

//Queue Oluşturma
//channel.QueueDeclare(queue:"example-queue", exclusive:false,durable:true, autoDelete:false); // exclusive false olması demek birden fazla channel tarafından bu kuyruğun üzerinde işlem yapılabilecek demektir.
//durable özelliğini true yapmamız bu kuyruğun kalıcı olmasını tanımladığını anlamına gelmektedir.Consumer tarafında da bu alan true olarak verilmelidir.
//autodelete: kuyruğa bağlı consumerlar eğer olmazsa otomatik olarak sil demek.


//Queue'ya Mesaj Gönderme

//RabbitMQ kuyruğa atacağı mesajları byte türünden kabul etmektedir. Haliyle mesajları(datları,istekleri) bizim byte'a dönüştürmemiz gerekecektir.

//byte'a dönüştürme örnek mesajı
//byte[] messageExample =   Encoding.UTF8.GetBytes("Merhaba");
//channel.BasicPublish(exchange:"", routingKey: "example-queue", body: messageExample); //default exchange direct exchange demektir. Şimdilik boş geçiyoruz, default kabul ediyoruz.
//routing-keydeki queue ismidir, yukarıda 17.satırda oluşturduğumuz kuyruğumuzdur.



#region Direct Exchange
//var exchangeDirectName = "logs-direct-exchange";

//var queuename = "direct-queue-infologs";
//var routingKey = "route-direct-log";

//channel.ExchangeDeclare(exchange: exchangeDirectName, durable:true,type:ExchangeType.Direct); //direct exchange tanımlama

//channel.QueueDeclare(queue:queuename,durable:true,autoDelete:false,exclusive:false); // queue'muzu oluşturma

//channel.QueueBind(queue:queuename,exchange: exchangeDirectName, routingKey: routingKey, null); //ilgili kuyruğumuzu exchange'e bind etme.


////bu properties instance'ı kuyrukta mesajlar için kalıcılık özelliğini tanımlamamızdır. Aşağıda basicpublish tarafında setleme işlemi yaparak kalıcı hale getiriyoruz.

//for (int i = 0; i < 5; i++)
//{
//    await Task.Delay(200);
//    byte[] messageExamples = Encoding.UTF8.GetBytes($"Direct log : {i}");
//    channel.BasicPublish(exchange: exchangeDirectName, routingKey: routingKey, body: messageExamples, basicProperties: properties);
//}
//// ayağa kalktığında https://moose.rmq.cloudamqp.com/#/connections sitesinde connect olduğunu görebileceğiz.
//Console.Read();
#endregion



#region Topic exchange 
//var exchangeTopicName = "logs-topic";
//var queueNameTopic = "topicQueue";
//var routingKeyTopic = "*.Error.*";

//channel.ExchangeDeclare(exchange: exchangeTopicName, durable: true, type: ExchangeType.Topic);
//channel.QueueDeclare(queue: queueNameTopic, durable: true, autoDelete: false, exclusive: false); // queue'muzu oluşturma

//channel.QueueBind(queue: queueNameTopic, exchange: exchangeTopicName, routingKey: routingKeyTopic, null);
//Random rnd = new();
//for (int i = 0; i < 50; i++)
//{
//    // "Critical.Info.Normal"; örnek bir mesajın routeKey
//    LogNames log1 = (LogNames)rnd.Next(1, 5);
//    LogNames log2 = (LogNames)rnd.Next(1, 5);
//    LogNames log3 = (LogNames)rnd.Next(1, 5);

//    var routeTopicKey = $"{log1}.{log2}.{log3}";
//    var messageTopic = $"Topic log-type : {log1}-{log2}-{log3}";
//    byte[] messageTopicExamples = Encoding.UTF8.GetBytes(messageTopic);
//    channel.BasicPublish(exchange: exchangeTopicName, routingKey: routeTopicKey, body: messageTopicExamples, basicProperties: properties);
//    Console.WriteLine($"Mesaj gönderilmiştir: {messageTopic}");

//}
//Console.Read();

#endregion

#region Header exchange

var exchangeHeaderName = "header-exchange";
channel.ExchangeDeclare(exchange: exchangeHeaderName, durable: true, type: ExchangeType.Headers);

Dictionary<string,object> headers = new Dictionary<string, object>();
headers.Add("format","pdf");
headers.Add("shape1","a4");
properties.Headers = headers;

byte[] messageHeaderExamples = Encoding.UTF8.GetBytes("Header örnek mesajım..");
channel.BasicPublish(exchange: exchangeHeaderName, routingKey: String.Empty, body: messageHeaderExamples, basicProperties: properties);
Console.WriteLine($"Mesaj gönderilmiştir.");

Console.Read();

#endregion
public enum LogNames
{
    Critical = 1,
    Info = 2,
    Warning = 3,
    Error = 4,
    Normal = 5
}