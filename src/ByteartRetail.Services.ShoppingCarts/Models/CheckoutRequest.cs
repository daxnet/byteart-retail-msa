using System.ComponentModel.DataAnnotations;

namespace ByteartRetail.Services.ShoppingCarts.Models;

public record CheckoutRequest([Required] Guid CustomerId);
