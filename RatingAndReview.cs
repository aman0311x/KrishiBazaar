using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KrishiBazaarProject.Models
{
    public class RatingAndReview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewID { get; set; }

        public int? ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        public string? BuyerID { get; set; }
        [ForeignKey("BuyerID")]
        public Users Buyer { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
