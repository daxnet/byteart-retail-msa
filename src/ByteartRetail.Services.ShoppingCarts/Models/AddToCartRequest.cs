using System.ComponentModel.DataAnnotations;

namespace ByteartRetail.Services.ShoppingCarts.Models
{
    public record AddToCartRequest(
        [Required] Guid? CustomerId, 
        [Required] Guid? ProductId,
        [Required] int? Quantity)
    {
    }
}
