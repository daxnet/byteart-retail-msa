namespace ByteartRetail.Common.Messaging;

public interface IEventHandler<in T>
    where T : IEvent
{
    Task<bool> HandleAsync(T @event, CancellationToken cancellationToken = default);
}