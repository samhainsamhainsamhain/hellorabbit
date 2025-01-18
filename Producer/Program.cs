using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

List<string> messages = GetMessages();

SendMessages(messages, channel);

Console.WriteLine("press Enter to exit");

static List<string> GetMessages()
{
    var rndm = new Random();
    List<string> messages = new List<string>();

    for(var i = 0; i < 200; i++)
    {
        var msg = "message number " + i + "..";

        for (var j = 0; j < rndm.Next(2, 10); j++)
        {
            msg += ".";
        }

        messages.Add(msg);
    }

    return messages;
}

static async void SendMessages(List<string> messages, IChannel channel)
{
    foreach(var message in messages)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: "logs", routingKey: string.Empty, body: body);
        Console.WriteLine(message);

        Thread.Sleep(400);
    }
}