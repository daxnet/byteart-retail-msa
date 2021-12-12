namespace ByteartRetail.Common.Messaging;

public interface IEvent
{
    public Guid Id { get; set; }

    public DateTime Timestamp { get; set; }
}