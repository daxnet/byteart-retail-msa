namespace ByteartRetail.Common.Messaging;

public abstract class EventBase : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}