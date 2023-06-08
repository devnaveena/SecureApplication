using System.ComponentModel.DataAnnotations;
using Entities.Helpher;

namespace Entities.Dtos.RequestDto
{
    public class UserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CustomDateValidation(ErrorMessage = "Invalid Date of Birth.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter a valid phone number")]
        public long Phone { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^(Reader|Admin)$", ErrorMessage = " Should  be either 'Reader' or 'Admin'")]
        public string Role { get; set; } = string.Empty;
        public string? Gender { get; set; }

    }


}
