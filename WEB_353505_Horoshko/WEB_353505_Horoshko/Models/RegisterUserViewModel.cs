using System.ComponentModel.DataAnnotations;

namespace WEB_353505_Horoshko.Models
{
    public class RegisterUserViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public IFormFile? Avatar { get; set; }
    }
}
