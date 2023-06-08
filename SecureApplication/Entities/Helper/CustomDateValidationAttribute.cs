using System.ComponentModel.DataAnnotations;

namespace Entities.Helpher
{
    public class CustomDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                // Perform custom date validation here
                if (dateOfBirth > DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}