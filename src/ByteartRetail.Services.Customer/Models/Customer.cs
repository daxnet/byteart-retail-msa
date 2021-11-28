using ByteartRetail.Common;

namespace ByteartRetail.Services.Customers.Models
{
    public class Customer : IEntity
    {
        public Customer(string name)
        {
            Name = name;
        }

        #region Public Properties

        public Guid Id { get; set; }

        public bool? IsActive { get; set; }

        public string Name { get; set; }

        public List<Address> Addresses { get; set; } = new();

        #endregion Public Properties

        #region Public Methods

        public override string ToString() => Name;

        #endregion Public Methods
    }
}
