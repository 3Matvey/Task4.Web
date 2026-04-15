using System.ComponentModel.DataAnnotations;

namespace Task4.Web.ViewModels
{
    public sealed class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(320)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string Password { get; set; } = string.Empty;
    }
}
