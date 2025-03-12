using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KrishiBazaar.Models
{
    public class ChatRoom
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BuyerId { get; set; }

        [Required]
        public string SellerId { get; set; }

        // ✅ Navigation Properties
        [ForeignKey("BuyerId")]
        public virtual Users Buyer { get; set; }

        [ForeignKey("SellerId")]
        public virtual Users Seller { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
