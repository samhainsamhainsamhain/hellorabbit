using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

int count = 0;

var factory = new ConnectionFactory {HostName = "localhost"};
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "hello", durable: true, exclusive: false, autoDelete: false,
    arguments: null);

await channel.BasicQosAsync(prefetchCount: 1, prefetchSize: 0, global: false);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine("received message: " + message);

    int dots = message.Split('.').Length - 1;

    await Task.Delay(dots * 100);

    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

    count++;
    Console.WriteLine("done " + count);
};

await channel.BasicConsumeAsync("hello", autoAck: false, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();