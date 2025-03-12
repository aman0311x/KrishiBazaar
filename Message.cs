using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KrishiBazaar.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChatRoomId { get; set; }

        [Required]
        public string SenderId { get; set; }  // 🔹 Buyer বা Seller যেকোনো একজন হতে পারে

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } // 🔹 ম্যাসেজের মূল কনটেন্ট

        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // 🔹 ম্যাসেজ পাঠানোর সময়

        // ✅ Navigation Properties
        [ForeignKey("ChatRoomId")]
        public virtual ChatRoom ChatRoom { get; set; }
    }
}
