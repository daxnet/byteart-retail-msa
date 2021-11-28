using ByteartRetail.Common;

namespace ByteartRetail.Services.ShoppingCarts.Models
{
    public class ShoppingCart : IEntity
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public List<LineItem> LineItems { get; set; } = new();
    }
}
