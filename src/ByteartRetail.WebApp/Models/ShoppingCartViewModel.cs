using System.Text.Json.Serialization;

namespace ByteartRetail.WebApp.Models;

public class ShoppingCartViewModel
{
    [JsonPropertyName("lineItems")]
    public List<ShoppingCartItemViewModel> Items { get; set; } = new();
}

public class ShoppingCartItemViewModel
{
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }
    
    [JsonPropertyName("productName")]
    public string ProductName { get; set; }
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    
    [JsonPropertyName("productPrice")]
    public float ProductPrice { get; set; }
    
    [JsonPropertyName("amount")]
    public float LineAmount { get; set; }
}