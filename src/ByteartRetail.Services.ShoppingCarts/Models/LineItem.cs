namespace ByteartRetail.Services.ShoppingCarts.Models
{
    public record LineItem(Guid ProductId, string ProductName, int Quantity)
    {
        public override string ToString() => ProductName;
    }
}
