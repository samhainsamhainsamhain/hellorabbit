﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [binding_key...]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

Console.WriteLine("Used topic: {0}", args);

int count = 1;

var factory = new ConnectionFactory {HostName = "localhost"};
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "topic_logs", type: ExchangeType.Topic);

QueueDeclareOk queueDeclareResult  = await channel.QueueDeclareAsync();
string queueName = queueDeclareResult.QueueName;

foreach (string? bindingKey in args)
{
    await channel.QueueBindAsync(queue: queueName, exchange: "topic_logs", routingKey: bindingKey);
}

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    byte[] body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}' --- '{count++}'");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();