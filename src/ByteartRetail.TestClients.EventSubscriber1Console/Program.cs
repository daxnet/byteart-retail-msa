// See https://aka.ms/new-console-template for more information

using ByteartRetail.Common.Messaging;
using ByteartRetail.Messaging.RabbitMQ;
using ByteartRetail.TestClients.Common;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;

var queueName = "byteartretail.subscriber1";
var routingKey = typeof(TextEvent).FullName;

if (args.Length == 1)
{
    routingKey = args[0];
}
else if (args.Length == 2)
{
    routingKey = args[0];
    queueName = args[1];
}

var eventHandlingContext = new DefaultEventHandlingContext();
eventHandlingContext.RegisterHandler<TextEvent, TextEventHandler>();
eventHandlingContext.RegisterHandler<RandomNumberEvent, RandomNumberEventHandler>();
var connectionFactory = new ConnectionFactory { HostName = "localhost" };

var messageSubscriber = new RabbitMQMessageSubscriber(
    eventHandlingContext,
    connectionFactory,
    "byteartretail.testclients",
    "direct",
    NullLogger<RabbitMQMessageSubscriber>.Instance);

messageSubscriber.Subscribe(routingKey, queueName);
Console.WriteLine("Subscriber1 started");
Console.ReadLine();

class TextEventHandler : IEventHandler<TextEvent>
{
    public Task<bool> HandleAsync(TextEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Text Received: {@event.Text}");
        return Task.FromResult(true);
    }
}

class RandomNumberEventHandler : IEventHandler<RandomNumberEvent>
{
    public Task<bool> HandleAsync(RandomNumberEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Number Value Received: {@event.Value}");
        return Task.FromResult(true);
    }
}
