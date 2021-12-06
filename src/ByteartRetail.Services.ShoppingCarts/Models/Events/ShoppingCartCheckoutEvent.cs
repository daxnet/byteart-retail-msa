using ByteartRetail.Common.Messaging;

namespace ByteartRetail.Services.ShoppingCarts.Models.Events;

public class ShoppingCartCheckoutEvent : IEvent
{
    public Guid Id { get; set; }
    public string RoutingKey { get; set; }
    public DateTime Timestamp { get; set; }
}