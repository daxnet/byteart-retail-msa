namespace ByteartRetail.Services.ShoppingCarts.Models
{
    public record ShoppingCartLineItem(Guid ProductId, string ProductName, float ProductPrice)
    {
        public int Quantity { get; set; }

        public float Amount { get; set; }

        public override string ToString() => ProductName;
    }
}
