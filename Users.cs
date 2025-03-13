using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KrishiBazaarProject.Models
{
    public class Users
    {
        [Key]
        public required String UserID { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string PhoneNumber { get; set; }
        public required bool IsFarmer { get; set; }
        public int? AddressID { get; set; }
        [ForeignKey("AddressID")]
        public Location Address { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Orders> Orders { get; set; } = new List<Orders>();
        public ICollection<UserMessages> MessagesSent { get; set; } = new List<UserMessages>();
        public ICollection<UserMessages> MessagesReceived { get; set; } = new List<UserMessages>();
    }
}
