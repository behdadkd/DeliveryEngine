using Engine.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

var timer = new Stopwatch();
timer.Start();

await Console.Out.WriteLineAsync(("Started =>" + timer.ElapsedMilliseconds));
int fullSize = 10000;
int batchSize = 1000;
var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };

using var _context = new ApplicationDbContext();
//var count = await _context.Entities.CountAsync();

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    //channel.CreateBasicPublishBatch(queue: "requests", durable: false, exclusive: false, autoDelete: false, arguments: null);

    for (int i = 0; i < fullSize; i = i + batchSize)
    {
        var entities = await _context.Entities.Skip(i).Take(batchSize).ToListAsync();
        var entityMessages = entities.Select(s => JsonSerializer.Serialize(s)).ToList();
        var batch = channel.CreateBasicPublishBatch();
        foreach (var m in entityMessages)
        {
            ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(m);
            batch.Add("", routingKey: "requests", false, null, body: body);
            //Console.WriteLine(" [x] Sent {0}", m);
        }
        await Console.Out.WriteLineAsync(($"batch {i} =>" + timer.ElapsedMilliseconds));
        batch.Publish();
        Console.WriteLine("Do :");
        Console.ReadLine();
    }
}

Console.WriteLine(" Press [enter] to exit.");
await Console.Out.WriteLineAsync(("Finish =>" + timer.ElapsedMilliseconds));
Console.ReadLine();


//var timer = new Stopwatch();
//timer.Start();

//using var _context = new ApplicationDbContext();
//var results = Enumerable.Range(0, 100000).Select(s => new Entity
//{
//    Number = new Random().Next(100000, 900000).ToString(),
//    Name = GenerateRandomString(100)
//});
//await _context.AddRangeAsync(results);
//await _context.SaveChangesAsync();
//await Console.Out.WriteLineAsync(("End =>" + timer.ElapsedMilliseconds));

//string GenerateRandomString(int length)
//{
//    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
//    return new string(Enumerable.Repeat(chars, length)
//      .Select(s => s[new Random().Next(s.Length)]).ToArray());
//}