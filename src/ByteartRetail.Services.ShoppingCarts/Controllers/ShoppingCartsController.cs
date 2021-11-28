using ByteartRetail.AspNetCore;
using ByteartRetail.Common.DataAccess;
using ByteartRetail.Services.ShoppingCarts.Models;
using Microsoft.AspNetCore.Mvc;

namespace ByteartRetail.Services.ShoppingCarts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartsController : DataServiceController<ShoppingCart>
    {
        public ShoppingCartsController(IDataAccessObject dao, ILogger<ShoppingCartsController> logger) : base(dao, logger)
        {
        }
    }
}
