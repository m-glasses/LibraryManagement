using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LibraryManagement.ViewModels.Validation
{
    public class IranianPhoneNumberAttribute : ValidationAttribute
    {
        private static readonly Regex Regex = new(
            @"^09\d{9}$",
            RegexOptions.Compiled);

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is not string phoneNumber)
            {
                return ValidationResult.Success;
            }

            return Regex.IsMatch(phoneNumber)
                ? ValidationResult.Success
                : new ValidationResult("فرمت وارد شده صحیح نیست");
        }
    }
}