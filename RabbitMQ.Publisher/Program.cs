

using RabbitMQ.Client;
using System.Text;

var connectionString = "amqps://tnywxfoe:2zNoaSpp5PH7xkHY-v6o9GqIa6yPPM-E@moose.rmq.cloudamqp.com/tnywxfoe";
var connectionStringDocker = "amqp://localhost:5672";
//Bağlantıyı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new (connectionStringDocker);

//Bağlantıyı Aktifleştirme ve Kanal Açma
using IConnection connection =  factory.CreateConnection(); //bağlantıyı aktifleştirdik.
using IModel channel =  connection.CreateModel(); // kanalı açtık.

//Queue Oluşturma
channel.QueueDeclare(queue:"example-queue", exclusive:false,durable:true, autoDelete:false); // exclusive false olması demek birden fazla channel tarafından bu kuyruğun üzerinde işlem yapılabilecek demektir.
//durable özelliğini true yapmamız bu kuyruğun kalıcı olmasını tanımladığını anlamına gelmektedir.Consumer tarafında da bu alan true olarak verilmelidir.
//autodelete: kuyruğa bağlı consumerlar eğer olmazsa otomatik olarak sil demek.


//Queue'ya Mesaj Gönderme

//RabbitMQ kuyruğa atacağı mesajları byte türünden kabul etmektedir. Haliyle mesajları(datları,istekleri) bizim byte'a dönüştürmemiz gerekecektir.

//byte'a dönüştürme örnek mesajı
//byte[] messageExample =   Encoding.UTF8.GetBytes("Merhaba");
//channel.BasicPublish(exchange:"", routingKey: "example-queue", body: messageExample); //default exchange direct exchange demektir. Şimdilik boş geçiyoruz, default kabul ediyoruz.
//routing-keydeki queue ismidir, yukarıda 17.satırda oluşturduğumuz kuyruğumuzdur.

IBasicProperties properties= channel.CreateBasicProperties();
properties.Persistent = true;
//bu properties instance'ı kuyrukta mesajlar için kalıcılık özelliğini tanımlamamızdır. Aşağıda basicpublish tarafında setleme işlemi yaparak kalıcı hale getiriyoruz.
//örnek mesela

for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] messageExample100 = Encoding.UTF8.GetBytes($"Merhaba: {i}");
    channel.BasicPublish(exchange: "", routingKey: "example-queue", body: messageExample100, basicProperties: properties);
}

Console.Read(); // ayağa kalktığında https://moose.rmq.cloudamqp.com/#/connections sitesinde connect olduğunu görebileceğiz.
