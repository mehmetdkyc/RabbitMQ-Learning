using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var connectionString = "amqp://localhost:5672"; //dockerdaki localhost bağlantısı rabbitmq
//Bağlantıyı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new(connectionString);

//Bağlantıyı Aktifleştirme ve Kanal Açma
using IConnection connection = factory.CreateConnection(); //bağlantıyı aktifleştirdik.
using IModel channel = connection.CreateModel(); // kanalı açtık.

channel.BasicQos(prefetchSize:0, prefetchCount:1,global:false); //global false olması her bir consumer için kaç tane gönderilmesi demek, true olması consumerlara toplam kaç adet olacağını hösterir. mesela 2 consumer var ve true dersek 0,5 arası belirlediğimizde birisine 3 birisine 2 tane mesaj yollayacak.
//yukarıdaki tanımlamada ilgili queueyu dinleye consumerlara sırayla 1 adet mesaj yolla demek.

var consumer = new EventingBasicConsumer(channel); //consumer oluşturduk 



#region Direct exchange consumer
//var exchangeDirectName = "logs-direct-exchange";

//var queuename = "direct-queue-infologs";
//var routingKey = "route-direct-log";

//channel.BasicConsume(queue: queuename, autoAck: false, consumer);
////autoAck false olması: kuyruktan eğer doğru işlenmesi durumunda haber verecek ve mesajı silecek, true olması işlem bittikten sonra doğru mu yanlış mı olmadan direkt silmesidir.

//consumer.Received += (sender, e) =>
//{
//    Thread.Sleep(1500);
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//    channel.BasicAck(e.DeliveryTag, false); // consumer mesajı başarılı aldığı için rabbitmqye bu mesajı silebilirsin diye bilgi veriyor.
//};
//Console.Read();

#endregion

#region Topic exchange cunsomer
//var exchangeTopicName = "logs-topic"; //istersek her iki taraftada queue ve exchangei oluşturabiliriz ama sadece publisher kısmında oluşturdum. Eğer aynı adta var ise yenisi oluşmuyor.

//var queueNameTopic = "topicQueue"; 
//var routingKey = "*.Error.*"; //topic route key için istediğimiz route key.

////publisher tarafında ilgili exchange ve kuyruğu oluşturup bind ettik
////ama garanti olması için burda da oluşturuyoruz eğer ki böyle bir kuyruk veuya exchange var ise önemlideğil oluşturmaz

//channel.ExchangeDeclare(exchange: exchangeTopicName, durable: true, type: ExchangeType.Topic);
//channel.QueueDeclare(queue: queueNameTopic, durable: true, autoDelete: false, exclusive: false); // queue'muzu oluşturma

//channel.QueueBind(queue: queueNameTopic, exchange: exchangeTopicName, routingKey: routingKey, null);

//channel.BasicConsume(queue: queueNameTopic, autoAck: false, consumer);
//Console.WriteLine("Loglar dinleniyor..");

//consumer.Received += (sender, e) =>
//{
//    Thread.Sleep(1500);
//    Console.WriteLine("Gelen mesaj: " +  Encoding.UTF8.GetString(e.Body.Span));
//    channel.BasicAck(e.DeliveryTag, false); // consumer mesajı başarılı aldığı için rabbitmqye bu mesajı silebilirsin diye bilgi veriyor.
//};
//Console.Read();
#endregion

#region Header Exchange

var exchangeHeaderName = "header-exchange";
var queueNameHeader = "headerQueue";

channel.ExchangeDeclare(exchange: exchangeHeaderName, durable: true, type: ExchangeType.Headers);
channel.QueueDeclare(queue: queueNameHeader, durable: true, autoDelete: false, exclusive: false); // queue'muzu oluşturma

Dictionary<string, object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a4");
headers.Add("x-match", "any"); // yukarıdaki key valuelardan birisini sağlar ise bu kuyrukta mesajları al demek. All olsaydı tüm key value türüne uyan mesajları işleme alacaktı.

channel.QueueBind(queue: queueNameHeader, exchange: exchangeHeaderName, routingKey: String.Empty, headers); // burda headerımızı queueya bind ediyoruz.

channel.BasicConsume(queue: queueNameHeader, autoAck: false, consumer);
Console.WriteLine("Loglar dinleniyor..");

consumer.Received += (sender, e) =>
{
    Thread.Sleep(1500);
    Console.WriteLine("Gelen mesaj: " + Encoding.UTF8.GetString(e.Body.ToArray()));
    channel.BasicAck(e.DeliveryTag, false); // consumer mesajı başarılı aldığı için rabbitmqye bu mesajı silebilirsin diye bilgi veriyor.
};
Console.Read();

#endregion