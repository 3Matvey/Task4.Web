using System.ComponentModel.DataAnnotations;

namespace Task4.Web.ViewModels
{
    public sealed class RegisterViewModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(320)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string Password { get; set; } = string.Empty;
    }
}
