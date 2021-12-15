using ByteartRetail.Common.Messaging;

namespace ByteartRetail.TestClients.Common;

public class RandomNumberEvent : EventBase
{
    public long Value { get; set; }
}