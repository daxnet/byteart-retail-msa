using ByteartRetail.AspNetCore;
using ByteartRetail.Common.DataAccess;
using ByteartRetail.Common.Messaging;
using ByteartRetail.Services.ShoppingCarts.Models;
using ByteartRetail.Services.ShoppingCarts.Models.Events;
using ByteartRetail.Services.ShoppingCarts.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace ByteartRetail.Services.ShoppingCarts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : DataServiceController<ShoppingCart>
    {
        private readonly ProductCatalogServiceClient _productCatalogServiceClient;
        private readonly IEventPublisher _eventPublisher;

        public ShoppingCartsController(
            IDataAccessObject dao, 
            IEventPublisher eventPublisher,
            ProductCatalogServiceClient productCatalogServiceClient,
            ILogger<ShoppingCartsController> logger) : base(dao, logger)
        {
            _productCatalogServiceClient = productCatalogServiceClient;
            _eventPublisher = eventPublisher;
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetShoppingCartByCustomerIdAsync(Guid customerId)
        {
            var shoppingCart = await Dao
                .GetAsync<ShoppingCart>(cart => cart.CustomerId == customerId, cart => cart.CustomerId)
                .ConfigureAwait(false);
            if (shoppingCart?.Count > 0)
            {
                return Ok(shoppingCart.First());
            }

            return NotFound();
        }

        [HttpPost]
        [Route("add-product")]
        public async Task<IActionResult> AddProductToCartAsync([FromBody] AddToCartRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0.");
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

            var lineItem = new ShoppingCartLineItem(request.ProductId.Value, product.Name, product.Price)
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

        [HttpPost("remove-product")]
        public async Task<IActionResult> RemoveProductFromCartAsync([FromBody] RemoveFromCartRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shoppingCarts = await Dao.GetAsync<ShoppingCart>(
                cart => cart.CustomerId == request.CustomerId,
                cart => cart.Id);
            if (shoppingCarts?.Count == 0)
            {
                return NotFound("The shopping cart doesn't exist.");
            }

            var shoppingCart = shoppingCarts!.First();
            shoppingCart.RemoveItem(request.ProductId);
            await Dao.UpdateByIdAsync(shoppingCart.Id, shoppingCart);
            return Ok(shoppingCart);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOutAsync([FromBody] CheckoutRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpGet("test-event/{routingKey}")]
        public async Task<IActionResult> TestEventPublishAsync(string routingKey )
        {
            await this._eventPublisher.PublishAsync(new ShoppingCartCheckoutEvent
            {
                Id = Guid.NewGuid(),
                RoutingKey = routingKey,
                Timestamp = DateTime.UtcNow
            });

            return Ok();
        }
    }
}
