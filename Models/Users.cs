using Microsoft.AspNetCore.Identity;

namespace KrishiBazaar.Models
{
    public class Users: IdentityUser
    {
        public string FullName { get; set; }
    }
}
