using System.ComponentModel.DataAnnotations;

namespace KrishiBazaar.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 4, ErrorMessage ="password")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not Match")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
