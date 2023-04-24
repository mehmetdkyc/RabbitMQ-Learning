

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

//Queue Oluşturma
//Consumer için rabbitmqden gelecek olan queueyu temsil edecek queue declare etmemiz gerekecektir.
channel.QueueDeclare(queue: "example-queue", exclusive: false,durable:true); // exclusive false olması demek birden fazla channel tarafından bu kuyruğun üzerinde işlem yapılabilecek demektir.
//consumer'da da kuyruk publisherdaki ile aynı yapılandırmada olmalıdır

//Queue'dan Mesaj Okuma

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue:"example-queue",autoAck:false,consumer); //queueya gelecek olan isteği tüketecek eventi oluşturuyorz. autoack kuyruktan alınan parametrenin silinip silinmeyeceğini belirleyen parametre
//ne zamanki ilgili queueye bir mesaj gelir onu consume et dedik ve bunu receive etmemiz gerekecek;
//autoAck:false olması kuyruktakı mesajın işlem neticesinde silinmesini karar veriyoruz.True olursa consumer mesajı aldıktan sonra başarılı mı tüketmiş başarısz mı ona bakmadan direkt kuyruktan silen bir olaydır. veri kaybına yol açar.
channel.BasicQos(0,1,false);
//ilk alan prefetchSize: burası bir consumer tarafından alınabilecek en büyük mesaj boyutunu byute cinsinden belirler.0,sınırsız demektir.
//ikinci alan prefetchCount: Bİr consumer tarafından aynı anda işleme alınabilecek mesaj sayısını belirler. Adil olması için 1 değerini verdik.
//son alan global: bu konfigurasyonun tüm consumerçlar için mi yoksa sadece çağrı yapılan consumer için mi geçerli olacağını belirler. false olması sadece bu consumer olduğunu belli eder.


consumer.Received += (sender, e) => //kuyruğa gelen mesajın işlendiği yerdir!!!
 {
     //e.Body : kuyruktaki mesajın verisini bütünsel olarak getirecektir
     //e.Body.Span veya e.body.toarray() : kuyruktaki mesajın byte verisini getirecektir.
     Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
     channel.BasicAck(deliveryTag:e.DeliveryTag,multiple:false); //rabbitmqya bu mesajın silinebileceğini belirtiyoruz. Başarılıysa kuyruktan silecek.
     //multiple false olunca sadece bu mesaja dair çalışma yap demek, true ise bu ve bundan öncekiler için silinme kararını vermiş oluyoruz.
 };

Console.Read(); // ayağa kalktığında https://moose.rmq.cloudamqp.com/#/connections sitesinde connect olduğunu görebileceğiz.

