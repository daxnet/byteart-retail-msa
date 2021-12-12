namespace ByteartRetail.Common.Messaging;

public interface IEventPublisher
{
    Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);

    event EventHandler Acknowledge;

    event EventHandler NegativeAcknowledge;
}