using ByteartRetail.AspNetCore;
using ByteartRetail.Common.DataAccess;
using ByteartRetail.Services.ProductCatalog.Models;
using Microsoft.AspNetCore.Mvc;

namespace ByteartRetail.Services.ProductCatalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCatalogController : DataServiceController<Product>
    {
        public ProductCatalogController(IDataAccessObject dao, ILogger<ProductCatalogController> logger) : base(dao, logger)
        {
        }
    }
}
