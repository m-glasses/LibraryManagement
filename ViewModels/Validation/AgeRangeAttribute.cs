using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.ViewModels.Validation
{
    public class AgeRangeAttribute : ValidationAttribute
    {
        private const int MinimumAge = 5;
        private const int MaximumAge = 110;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime dateOfBirth)
            {
                return ValidationResult.Success;
            }

            DateTime today = DateTime.Today;

            DateTime youngestDate = today.AddYears(-MinimumAge);
            DateTime oldestDate = today.AddYears(-MaximumAge);

            if (dateOfBirth < oldestDate || dateOfBirth > youngestDate)
            {
                return new ValidationResult("سن باید بین ده تا صدوده سال باشد");
            }

            return ValidationResult.Success;

        }


    }
}
