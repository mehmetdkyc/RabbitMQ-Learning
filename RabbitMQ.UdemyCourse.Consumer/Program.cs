using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var connectionString = "amqp://localhost:5672";
//Bağlantıyı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new(connectionString);

//Bağlantıyı Aktifleştirme ve Kanal Açma
using IConnection connection = factory.CreateConnection(); //bağlantıyı aktifleştirdik.
using IModel channel = connection.CreateModel(); // kanalı açtık.

channel.BasicQos(prefetchSize:0, prefetchCount:1,global:false); //global false olması her bir consumer için kaç tane gönderilmesi demek, true olması consumerlara toplam kaç adet olacağını hösterir. mesela 2 consumer var ve true dersek 0,5 arası belirlediğimizde birisine 3 birisine 2 tane mesaj yollayacak.
//yukarıdaki tanımlamada ilgili queueyu dinleye consumerlara sırayla 1 adet mesaj yolla demek.

var consumer = new EventingBasicConsumer(channel); //consumer oluşturduk 

channel.BasicConsume(queue: "example-queue",autoAck:false,consumer);
//autoAck false olması: kuyruktan eğer doğru işlenmesi durumunda haber verecek ve mesajı silecek, true olması işlem bittikten sonra doğru mu yanlış mı olmadan direkt silmesidir.

consumer.Received += (sender, e) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    channel.BasicAck(e.DeliveryTag,false); // consumer mesajı başarılı aldığı için rabbitmqye bu mesajı silebilirsin diye bilgi veriyor.
};
Console.Read();