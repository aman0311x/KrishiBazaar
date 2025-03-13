using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KrishiBazaar.Models;


namespace KrishiBazaar.ViewModels
{
    public class CheckoutViewModel
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SellerName { get; set; }
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
        public string SellerDivision { get; set; }

        [Required]
        public int SelectedQuantity { get; set; }

        [Required]
        public int SelectedHubId { get; set; }

        public List<Hub> Hubs { get; set; }
    }
}
