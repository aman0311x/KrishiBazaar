using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KrishiBazaarProject.Models
{
    public class UserMessages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageID { get; set; }

        public string? SenderID { get; set; }
        [ForeignKey("SenderID")]
        public Users Sender { get; set; }

        public string? ReceiverID { get; set; }
        [ForeignKey("ReceiverID")]
        public Users Receiver { get; set; }

        public required string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
