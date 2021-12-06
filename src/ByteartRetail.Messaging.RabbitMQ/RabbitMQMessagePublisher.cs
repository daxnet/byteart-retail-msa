using System.Text.Json;
using System.Text.Json.Serialization;
using ByteartRetail.Common.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ByteartRetail.Messaging.RabbitMQ;

public class RabbitMQMessagePublisher : RabbitMQMessagingBase, IEventPublisher
{
    public RabbitMQMessagePublisher(
        IConnectionFactory connectionFactory, 
        string exchangeName, 
        string exchangeType, 
        ILogger<RabbitMQMessagePublisher> logger) : base(connectionFactory, exchangeName, exchangeType, logger)
    {
    }

    public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType());
        Channel.BasicPublish(ExchangeName, @event.RoutingKey, null, payload);
        return Task.CompletedTask;
    }
}