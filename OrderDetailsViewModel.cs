namespace KrishiBazaar.ViewModels
{
    public class OrderDetailsViewModel
    {
        public string ProductName { get; set; }
        public string SellerName { get; set; }
        public string SellerAddress { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public string ImagePath { get; set; } // New: Store image path
    }
}
