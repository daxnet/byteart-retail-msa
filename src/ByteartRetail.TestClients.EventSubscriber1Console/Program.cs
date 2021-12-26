// See https://aka.ms/new-console-template for more information

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

eventSubscriber.Subscribe("customers");
Console.WriteLine("customers service started");
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
            case "check-customer-validity":
                var customerName = @event.Payload.Split('=')[1];
                Console.WriteLine($"  -> Customer name: {customerName}");
                @event.Succeeded = customerName == "daxnet";
                if (!@event.Succeeded)
                {
                    @event.FailedReason = "Customer name is not daxnet";
                }
                
                Console.WriteLine($"check-customer-validity: Succeeded: {@event.Succeeded}, Failed Reason: {@event.FailedReason}");
                break;
            case "reserve-credit":
                if (int.TryParse(@event.Payload.Split('=')[1], out var creditValue))
                {
                    Console.WriteLine($"  -> Credit value: {creditValue}");
                    @event.Succeeded = creditValue <= 1000;
                    if (!@event.Succeeded)
                    {
                        @event.FailedReason = "Credit exceeded the allowed limit.";
                    }
                }
                else
                {
                    @event.Succeeded = false;
                    @event.FailedReason = "Credit value is not in a correct format.";
                }

                Console.WriteLine($"reserve-credit: Succeeded: {@event.Succeeded}, Failed Reason: {@event.FailedReason}");
                break;
            case "compensate-reserve-credit":
                if (int.TryParse(@event.Payload.Split('=')[1], out var val))
                {
                    Console.WriteLine($"  -> Compensated with the amount of {val}");
                    @event.Succeeded = true;
                }

                Console.WriteLine($"compensate-reserve-credit: Succeeded: {@event.Succeeded}, Failed Reason: {@event.FailedReason}");
                break;
        }

        await _messagePublisher.PublishAsync(@event, "shopping-cart");
        return true;
    }
}