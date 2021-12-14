using System.Collections.Concurrent;

namespace ByteartRetail.Common.Messaging;

public class DefaultEventHandlingContext : IEventHandlingContext
{
    protected readonly ConcurrentDictionary<Type, List<Type>> _eventHandlerRegistration = new();
    
    public virtual void RegisterHandler<TEvent, TEventHandler>() where TEvent : IEvent where TEventHandler : IEventHandler<TEvent>
    {
        if (_eventHandlerRegistration.TryGetValue(typeof(TEvent), out var handlerTypes))
        {
            if (!handlerTypes.Contains(typeof(TEventHandler)))
            {
                _eventHandlerRegistration[typeof(TEvent)].Add(typeof(TEventHandler));
            }
        }
        else
        {
            _eventHandlerRegistration.TryAdd(typeof(TEvent), new List<Type> { typeof(TEventHandler) });
        }
    }

    public virtual async Task<bool> HandleEventAsync(IEvent evnt, CancellationToken cancellationToken = default)
    {
        var eventType = evnt.GetType();
        if (!_eventHandlerRegistration.TryGetValue(eventType, out var handlerTypes) ||
            handlerTypes.Count == 0) return false;
        
        var handled = true;
        foreach (var handlerType in handlerTypes)
        {
            var handlerInstance = Activator.CreateInstance(handlerType);
            var methodInfo = handlerType.GetMethod("HandleAsync", new[] { eventType, typeof(CancellationToken) });
            if (handlerInstance != null && methodInfo != null)
            {
                var returnValue = methodInfo.Invoke(handlerInstance, new object[] { evnt, cancellationToken }) as Task<bool>;
                handled &= await (returnValue ?? Task.FromResult(false));
            }
        }

        return handled;
    }
}