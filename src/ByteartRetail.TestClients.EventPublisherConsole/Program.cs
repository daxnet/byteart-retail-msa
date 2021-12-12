// See https://aka.ms/new-console-template for more information

using System.Runtime.Loader;
using ByteartRetail.Messaging.RabbitMQ;
using ByteartRetail.TestClients.Common;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;

var connectionFactory = new ConnectionFactory { HostName = "localhost" };
var messagePublisher = new RabbitMQMessagePublisher(
    connectionFactory, 
    "byteartretail.testclients", 
    "direct", 
    NullLogger<RabbitMQMessagePublisher>.Instance);

messagePublisher.Acknowledge += (sender, args) =>
{
    Console.WriteLine("Message sent.");
};

messagePublisher.NegativeAcknowledge += (sender, args) =>
{
    Console.WriteLine("Message failed to send.");
};

var evnt = new MessageEvent { Message = "Hello, World!" };
await messagePublisher.PublishAsync(evnt);
