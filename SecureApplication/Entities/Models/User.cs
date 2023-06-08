using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("User")]
    public class User : CommonModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public long Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
    }
}