// See https://aka.ms/new-console-template for more information

using System.Runtime.Loader;
using ByteartRetail.Common.Messaging;
using ByteartRetail.DataAccess.Mongo;
using ByteartRetail.Messaging.RabbitMQ;
using ByteartRetail.TestClients.Common;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using RabbitMQ.Client;

var connectionFactory = new ConnectionFactory { HostName = "localhost" };
var messagePublisher = new RabbitMQMessagePublisher(
    connectionFactory, 
    "byteartretail.testclients", 
    "direct", 
    NullLogger<RabbitMQMessagePublisher>.Instance);

var dao = new MongoDataAccessObject(MongoUrl.Create(""), "sagas");

while (true)
{
    Console.Write("Enter a numeric or text value: ");
    var input = Console.ReadLine();
    if (string.IsNullOrEmpty(input))
        break;
    IEvent evnt;
    if (int.TryParse(input, out var v))
    {
        evnt = new RandomNumberEvent { Value = v };
    }
    else
    {
        evnt = new TextEvent { Text = input };
    }

    await messagePublisher.PublishAsync(evnt);
}
