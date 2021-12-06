using ByteartRetail.Common;

namespace ByteartRetail.Services.ShoppingCarts.Models
{
    public class ShoppingCart : IEntity
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public List<ShoppingCartLineItem> LineItems { get; set; } = new();

        public bool AddItem(ShoppingCartLineItem item)
        {
            if (item.Quantity > 0)
            {
                if (!LineItems.Any(li => li.ProductId == item.ProductId))
                {
                    LineItems.Add(item);
                }
                else
                {
                    var existingItem = LineItems.First(li => li.ProductId == item.ProductId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += item.Quantity;
                        existingItem.Amount += item.Amount;
                    }
                }
                return true;
            }

            return false;
        }

        public bool RemoveItem(Guid productId)
        {
            var idx = LineItems.FindIndex(li => li.ProductId == productId);
            if (idx >= 0)
            {
                LineItems.RemoveAt(idx);
                return true;
            }

            return false;
        }
    }
}
