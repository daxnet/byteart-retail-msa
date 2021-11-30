using ByteartRetail.Common;

namespace ByteartRetail.Services.Customers.Models
{
    public class Customer : IEntity
    {
        public Customer(string name, string email, string nickName)
        {
            Name = name;
            Email = email;
            NickName = nickName;
        }

        #region Public Properties

        public Guid Id { get; set; }

        public bool? IsActive { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string NickName { get; set; }

        public List<Address> Addresses { get; set; } = new();

        #endregion Public Properties

        #region Public Methods

        public override string ToString() => Name;

        #endregion Public Methods
    }
}
