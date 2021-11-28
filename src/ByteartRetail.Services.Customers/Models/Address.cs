namespace ByteartRetail.Services.Customers.Models
{
    /// <summary>
    /// Represents the address of a customer.
    /// </summary>
    /// <param name="State"></param>
    /// <param name="City"></param>
    /// <param name="Street"></param>
    /// <param name="AddressLine"></param>
    /// <param name="ZipCode"></param>
    public record Address (string State, string City, string Street, string AddressLine, string ZipCode)
    {
        /// <summary>
        /// Validates the current address.
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return !string.IsNullOrEmpty(State) &&
                !string.IsNullOrEmpty(City) &&
                !string.IsNullOrEmpty(Street) &&
                !string.IsNullOrEmpty(AddressLine) &&
                !string.IsNullOrEmpty(ZipCode);
        }
    }
}
