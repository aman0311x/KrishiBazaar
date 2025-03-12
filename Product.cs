using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KrishiBazaar.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string ProductType { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string QuantityUnit { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? ImagePath { get; set; } // Store relative image path

        [Required]
        public string FarmerId { get; set; } // ✅ Add this field

        [ForeignKey("FarmerId")]
        public Users Farmer { get; set; } // ✅ Navigation property
    }
}
