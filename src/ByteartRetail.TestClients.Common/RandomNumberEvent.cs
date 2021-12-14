using ByteartRetail.Common.Messaging;

namespace ByteartRetail.TestClients.Common;

public class RandomNumberEvent : EventBase
{
    public int Value { get; set; }
}