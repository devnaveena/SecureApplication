using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.RequestDto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "This field is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        public string Password { get; set; } = string.Empty;
    }
}