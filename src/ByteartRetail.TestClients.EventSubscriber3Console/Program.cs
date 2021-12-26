using ByteartRetail.Common.Messaging;
using ByteartRetail.Messaging.RabbitMQ;
using ByteartRetail.TestClients.Common.Sagas;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;

var eventHandlingContext = new DefaultEventHandlingContext();
eventHandlingContext.RegisterHandler<SagaEvent, SagaEventHandler>();
var connectionFactory = new ConnectionFactory { HostName = "localhost" };

var eventSubscriber = new RabbitMQMessageSubscriber(
    eventHandlingContext,
    connectionFactory,
    "sagaExchange",
    "direct",
    NullLogger<RabbitMQMessageSubscriber>.Instance);

eventSubscriber.Subscribe("sales-order");
Console.WriteLine("Sales order service started");
Console.ReadLine();

class SagaEventHandler : IEventHandler<SagaEvent>
{
    private readonly RabbitMQMessagePublisher _messagePublisher;

    public SagaEventHandler()
    {
        var connectionFactory = new ConnectionFactory { HostName = "localhost" };
        _messagePublisher = new RabbitMQMessagePublisher(
            connectionFactory,
            "sagaReturnExchange",
            "direct",
            NullLogger<RabbitMQMessagePublisher>.Instance
        );
    }
    
    public async Task<bool> HandleAsync(SagaEvent @event, CancellationToken cancellationToken = default)
    {
        @event.Succeeded = true;

        Console.WriteLine($"{@event.EventType} Succeeded: {@event.Succeeded}, Failed Reason: {@event.FailedReason}");
        await _messagePublisher.PublishAsync(@event, "shopping-cart");
        return true;
    }
}