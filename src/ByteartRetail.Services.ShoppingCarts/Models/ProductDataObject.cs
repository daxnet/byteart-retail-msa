namespace ByteartRetail.Services.ShoppingCarts.Models
{
    public record ProductDataObject(Guid Id, string Name, string Description, float Price, int Inventory);
}
