using ByteartRetail.Common.DataAccess;
using ByteartRetail.Services.ShoppingCarts.Models;
using System.Text.Json;

namespace ByteartRetail.Services.ShoppingCarts.ServiceClients
{
    public sealed class ProductCatalogServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductCatalogServiceClient> _logger;

        public ProductCatalogServiceClient(
            HttpClient httpClient,
            ILogger<ProductCatalogServiceClient> logger) =>
            (_httpClient, _logger) = (httpClient, logger);

        public async Task<ProductDataObject?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"api/ProductCatalog/{productId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            var productJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var productObject = JsonSerializer.Deserialize<ProductDataObject>(productJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return productObject;
        }
    }
}
