namespace KrishiBazaar.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }
}
