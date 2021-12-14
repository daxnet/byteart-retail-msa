using System.Text;
using System.Text.Json;
using ByteartRetail.Common.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ByteartRetail.Messaging.RabbitMQ;

public sealed class RabbitMQMessageSubscriber : RabbitMQMessagingBase, IEventSubscriber
{
    private const string EventWasNotHandledErrorMessage = "Event was not handled. Event ID: {0}";
    private readonly EventingBasicConsumer _consumer;
    private readonly IEventHandlingContext _eventHandlingContext;

    public RabbitMQMessageSubscriber(
        IEventHandlingContext eventHandlingContext,
        IConnectionFactory connectionFactory, 
        string exchangeName, 
        string exchangeType, 
        ILogger<RabbitMQMessageSubscriber> logger) : base(connectionFactory, exchangeName, exchangeType, logger)
    {
        _eventHandlingContext = eventHandlingContext;
        _consumer = new EventingBasicConsumer(Channel);
        _consumer.Received += ConsumerOnReceived;
    }

    private async void ConsumerOnReceived(object? sender, BasicDeliverEventArgs e)
    {
        var messageBody = e.Body;
        var json = Encoding.UTF8.GetString(messageBody.ToArray());
        // var eventData = JsonSerializer.Deserialize<EventData>(json);
        var eventData = JsonConvert.DeserializeObject<EventData>(json);
        if (eventData == null)
        {
            return;
        }
        var evnt = eventData.AsEvent();
        if (evnt == null)
        {
            return;
        }
        
        try
        {
            var handlingResult = await _eventHandlingContext.HandleEventAsync(evnt);
            if (handlingResult)
            {
                Channel.BasicAck(e.DeliveryTag, false);
            }
            else
            {
                Channel.BasicNack(e.DeliveryTag, false, true);
            }
        }
        catch (Exception exception)
        {
            Logger.LogWarning(string.Format(EventWasNotHandledErrorMessage, eventData?.EventId));
            Logger.LogWarning(exception.ToString());
            Channel.BasicNack(e.DeliveryTag, false, true);
        }
    }

    public void Subscribe(string routingKey, string? queueName = null)
    {
        if (string.IsNullOrEmpty(queueName))
        {
            queueName = Channel.QueueDeclare().QueueName;
        }
        else
        {
            Channel.QueueDeclare(queueName, exclusive: false);
        }

        Channel.QueueBind(queueName, ExchangeName, routingKey);
        Channel.BasicConsume(queueName, false, _consumer);
    }
}