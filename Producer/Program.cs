using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "direct_logs", type: ExchangeType.Direct);

List<string> severityTypes = new List<string> { "info", "warning", "error" };
List<string> messages = GetMessages(severityTypes);

SendMessages(messages, channel);

Console.WriteLine("press Enter to exit");

static List<string> GetMessages(List<string> severityTypes)
{
    var rndm = new Random();
    List<string> messages = new List<string>();

    for(var i = 0; i < 500; i++)
    {
        messages.Add(severityTypes[rndm.Next(0, 3)]);
    }

    return messages;
}

static async void SendMessages(List<string> messages, IChannel channel)
{
    foreach (var message in messages)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: "direct_logs", routingKey: message, body: body);
        Console.WriteLine(message);

        Thread.Sleep(200);
    }
}