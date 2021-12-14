using ByteartRetail.Common.Messaging;

namespace ByteartRetail.TestClients.Common;

public class TextEvent : EventBase
{
    public string? Text { get; set; }
}