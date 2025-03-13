using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KrishiBazaarProject.Models
{
    public class Delivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeliveryID { get; set; }

        public int? OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Orders Order { get; set; }

        public int? CurrentLocationID { get; set; }
        [ForeignKey("CurrentLocationID")]
        public Location Locations { get; set; }

    }
}
