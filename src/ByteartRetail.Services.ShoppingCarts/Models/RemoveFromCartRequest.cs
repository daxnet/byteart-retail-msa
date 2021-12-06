using System.ComponentModel.DataAnnotations;

namespace ByteartRetail.Services.ShoppingCarts.Models;

public record RemoveFromCartRequest(
    [Required] Guid CustomerId,
    [Required] Guid ProductId);