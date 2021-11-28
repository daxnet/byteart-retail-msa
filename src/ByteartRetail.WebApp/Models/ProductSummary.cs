namespace ByteartRetail.WebApp.Models
{
    public class ProductSummary
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int Inventory { get; set; }

        public string? Description { get; set; }

        public float Price { get; set; }
    }
}
