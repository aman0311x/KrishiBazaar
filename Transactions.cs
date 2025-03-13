using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KrishiBazaarProject.Models
{
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }

        public int? OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Orders Order{ get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        public bool PaymentStatus { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

    }

}
