namespace ByteartRetail.Common.Messaging;

public interface IEventPublisher
{
    Task PublishAsync(IEvent @event, string? routingKey = null, CancellationToken cancellationToken = default);

    event EventHandler Acknowledge;

    event EventHandler NegativeAcknowledge;
}