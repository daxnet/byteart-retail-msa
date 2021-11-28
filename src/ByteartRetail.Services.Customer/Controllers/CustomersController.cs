using ByteartRetail.AspNetCore;
using ByteartRetail.Common.DataAccess;
using ByteartRetail.Services.Customers.Models;
using Microsoft.AspNetCore.Mvc;


namespace ByteartRetail.Services.Customers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : DataServiceController<Customer>
    {
        public CustomersController(IDataAccessObject dao, ILogger<CustomersController> logger) : base(dao, logger)
        {
        }

        [HttpPost("{customerId}/addresses")]
        public async Task<IActionResult> AddAddressAsync([FromRoute] Guid customerId, [FromBody] Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await Dao.GetByIdAsync<Customer>(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            customer.Addresses.Add(address);
            await Dao.UpdateByIdAsync(customerId, customer);

            return Ok(customer);
        }
    }
}
