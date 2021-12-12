namespace ByteartRetail.Common.Messaging;

public interface IEventHandlingContext
{
    void RegisterHandler<TEvent, TEventHandler>()
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>;

    Task<bool> HandleEventAsync(IEvent evnt, CancellationToken cancellationToken = default);
}