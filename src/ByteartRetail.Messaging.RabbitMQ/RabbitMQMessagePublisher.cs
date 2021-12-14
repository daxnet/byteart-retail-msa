using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ByteartRetail.Common.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ByteartRetail.Messaging.RabbitMQ;

public class RabbitMQMessagePublisher : RabbitMQMessagingBase, IEventPublisher
{
    public RabbitMQMessagePublisher(
        IConnectionFactory connectionFactory, 
        string exchangeName, 
        string exchangeType, 
        ILogger<RabbitMQMessagePublisher> logger) : base(connectionFactory, exchangeName, exchangeType, logger)
    {
        Channel.ConfirmSelect();
        Channel.BasicAcks += ChannelOnBasicAcks;
        Channel.BasicNacks += ChannelOnBasicNacks;
    }

    private void ChannelOnBasicNacks(object? sender, BasicNackEventArgs e)
    {
        NegativeAcknowledge?.Invoke(sender, e);
    }

    private void ChannelOnBasicAcks(object? sender, BasicAckEventArgs e)
    {
        Acknowledge?.Invoke(sender, e);
    }

    public Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        var eventData = new EventData(@event);
        // var payload = JsonSerializer.SerializeToUtf8Bytes(eventData, typeof(EventData));
        var payloadJson = JsonConvert.SerializeObject(eventData, Formatting.None);
        var payload = Encoding.UTF8.GetBytes(payloadJson);
        Channel.BasicPublish(ExchangeName, @event.GetType().FullName, null, payload);
        return Task.CompletedTask;
    }

    public event EventHandler? Acknowledge;
    public event EventHandler? NegativeAcknowledge;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Channel.BasicAcks -= ChannelOnBasicAcks;
            Channel.BasicNacks -= ChannelOnBasicNacks;
        }
        base.Dispose(disposing);
    }
}