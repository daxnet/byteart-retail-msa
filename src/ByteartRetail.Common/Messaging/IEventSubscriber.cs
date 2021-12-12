namespace ByteartRetail.Common.Messaging;

public interface IEventSubscriber
{
    void Subscribe(string routingKey, string? queueName = null);
}