// Create RabbitMQ connection
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

var timer = new Stopwatch();
timer.Start();
await Console.Out.WriteLineAsync(("Started => Time : " + timer.ElapsedMilliseconds));

var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };


using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "requests", durable: false, exclusive: false, autoDelete: false, arguments: null);
var hitCount = 0;

using (HttpClient httpClient = new())
{

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += async (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        //Console.WriteLine(" [x] Received {0}", message);

        var response = await httpClient.PostAsync($"http://localhost:12345/delayMessage?message={message}", null);
        hitCount++;
    };
    channel.BasicConsume(queue: "requests", autoAck: true, consumer: consumer);


    await Console.Out.WriteLineAsync(("End => Time : " + timer.ElapsedMilliseconds) + $"Hit : {hitCount}");

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}