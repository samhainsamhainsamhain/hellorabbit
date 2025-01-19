using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);

List<string> colorTypes = new List<string> { "red", "blue", "green" };
List<string> speedTypes = new List<string> { "slow", "normal", "fast" };
List<string> animalTypes = new List<string> { "turtle", "dog", "cat" };

List<string> messages = GetMessages(colorTypes, speedTypes, animalTypes);

SendMessages(messages, channel);

Console.WriteLine("press Enter to exit");

static List<string> GetMessages(List<string> colorTypes, List<string> speedTypes, List<string> animalTypes)
{
    var rndm = new Random();
    List<string> messages = new List<string>();

    for(var i = 0; i < 500; i++)
    {
        messages.Add($"{colorTypes[rndm.Next(0, 3)]}.{speedTypes[rndm.Next(0, 3)]}.{animalTypes[rndm.Next(0, 3)]}");
    }

    return messages;
}

static async void SendMessages(List<string> messages, IChannel channel)
{
    foreach (var message in messages)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: "topic_logs", routingKey: message, body: body);
        Console.WriteLine(message);

        Thread.Sleep(200);
    }
}