// Create RabbitMQ connection
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

var timer = new Stopwatch();
timer.Start();
await Console.Out.WriteLineAsync(("Started => Time : " + timer.ElapsedMilliseconds));

BindingList<Task> messageTasks = new();

var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
factory.DispatchConsumersAsync = true;



using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "requests", durable: false, exclusive: false, autoDelete: false, arguments: null);

using (HttpClient httpClient = new())
{
    var consumer = new AsyncEventingBasicConsumer(channel);

    consumer.Received += async (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        messageTasks.Add(MakeRequestAsync(httpClient, message));
        channel.BasicAck(ea.DeliveryTag, false);

        //await Console.Out.WriteLineAsync(("Hit => Time : " + timer.ElapsedMilliseconds));
    };

    messageTasks.ListChanged += async (sender, e) =>
    {
        if (messageTasks.Count == 5)
        {
            await Task.WhenAll(messageTasks);
            messageTasks.Clear();
            Console.WriteLine("5 EXE");
        }
    };

    channel.BasicConsume(queue: "requests", autoAck: false, consumer: consumer);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();

    static async Task MakeRequestAsync(HttpClient client, string message)
    {
        await client.PostAsync($"http://localhost:12345/delayMessage?message={message}", null);
    }
}