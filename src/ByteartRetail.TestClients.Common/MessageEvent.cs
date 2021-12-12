using ByteartRetail.Common.Messaging;

namespace ByteartRetail.TestClients.Common;

public class MessageEvent : EventBase
{
    public string? Message { get; set; }
}