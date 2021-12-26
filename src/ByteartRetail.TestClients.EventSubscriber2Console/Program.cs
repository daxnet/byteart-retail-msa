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

eventSubscriber.Subscribe("product-catalog");
Console.WriteLine("product-catalog service started");
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
        switch (@event.EventType)
        {
            case "reserve-inventory":
                if (int.TryParse(@event.Payload.Split('=')[1], out var reservingAmount))
                {
                    Console.WriteLine($" -> Request for inventory: {reservingAmount}");
                    @event.Succeeded = reservingAmount <= 5000;
                    if (!@event.Succeeded)
                    {
                        @event.FailedReason = $"The inventory doesn't have the number claimed by the amount of {reservingAmount}.";
                    }
                }
                else
                {
                    @event.Succeeded = false;
                    @event.FailedReason = "Reserving Amount is not in a correct format.";
                }
                break;
        }

        Console.WriteLine($"reserve-inventory succeeded: {@event.Succeeded}, Failed Reason: {@event.FailedReason}");
        await _messagePublisher.PublishAsync(@event, "shopping-cart");
        return true;
    }
}