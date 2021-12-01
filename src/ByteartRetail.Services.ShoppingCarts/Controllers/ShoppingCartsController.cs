using ByteartRetail.AspNetCore;
using ByteartRetail.Common.DataAccess;
using ByteartRetail.Services.ShoppingCarts.Models;
using ByteartRetail.Services.ShoppingCarts.ServiceClients;
using Microsoft.AspNetCore.Mvc;

namespace ByteartRetail.Services.ShoppingCarts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : DataServiceController<ShoppingCart>
    {
        private readonly ProductCatalogServiceClient _productCatalogServiceClient;

        public ShoppingCartsController(
            IDataAccessObject dao, 
            ProductCatalogServiceClient productCatalogServiceClient,
            ILogger<ShoppingCartsController> logger) : base(dao, logger)
        {
            _productCatalogServiceClient = productCatalogServiceClient;
        }

        [HttpPost]
        [Route("add-product")]
        public async Task<IActionResult> AddProductToCartAsync([FromBody] AddToCartRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shoppingCart = (await Dao
                .GetAsync<ShoppingCart>(cart => cart.CustomerId == request.CustomerId, sort => sort.CustomerId)
                .ConfigureAwait(false))
                .FirstOrDefault();
            var product = await _productCatalogServiceClient.GetProductByIdAsync(request.ProductId!.Value);
            if (product == null)
            {
                return NotFound($"Product {request.ProductId} doesn't exist.");
            }

            var lineItem = new ShoppingCartLineItem(request.ProductId.Value, product.Name)
            {
                Quantity = request.Quantity!.Value,
                Amount = product.Price * request.Quantity!.Value
            };

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart { CustomerId = request.CustomerId!.Value };
                shoppingCart.AddItem(lineItem);
                await Dao.AddAsync(shoppingCart);
            }
            else
            {
                shoppingCart.AddItem(lineItem);
                await Dao.UpdateByIdAsync(shoppingCart.Id, shoppingCart);
            }

            return Ok(shoppingCart);
        }
    }
}
