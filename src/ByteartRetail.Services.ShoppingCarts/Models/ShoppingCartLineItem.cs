namespace ByteartRetail.Services.ShoppingCarts.Models
{
    public record ShoppingCartLineItem(Guid ProductId, string ProductName)
    {
        public int Quantity { get; set; }

        public float Amount { get; set; }

        public override string ToString() => ProductName;
    }
}
