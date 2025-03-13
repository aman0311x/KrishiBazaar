namespace KrishiBazaarProject.ViewModels
{
    public class ProductDetailsViewModel
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public int AvailableQuantity { get; set; }
        public string Photos { get; set; }
        public dynamic Farmer { get; set; }
        public double AverageRating { get; set; }
        public List<ReviewViewModel>? Reviews { get; set; }
    }

    public class ReviewViewModel
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string BuyerName { get; set; }
    }
}
