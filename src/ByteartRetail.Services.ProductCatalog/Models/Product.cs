using ByteartRetail.Common;

namespace ByteartRetail.Services.ProductCatalog.Models
{
    public class Product : IEntity
    {
        public Product()
            : this("Unnamed Product")
        { }

        public Product(string name)
        { 
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime? ValidThrough { get; set; }

        public float Price { get; set; }

        public int Inventory { get; set; }

        public override string ToString() => Name;
    }
}
