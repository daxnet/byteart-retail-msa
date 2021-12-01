using ByteartRetail.Services.ShoppingCarts.Models;

namespace ByteartRetail.Services.ShoppingCarts.ServiceClients
{
    public sealed class ProductCatalogServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductCatalogServiceClient> _logger;

        public ProductCatalogServiceClient(HttpClient httpClient, ILogger<ProductCatalogServiceClient> logger) =>
            (_httpClient, _logger) = (httpClient, logger);

        public async Task<ShoppingCart> GetShoppingCartByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {

        }
    }
}
