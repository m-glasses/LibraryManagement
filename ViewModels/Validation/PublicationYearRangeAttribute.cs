using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace LibraryManagement.ViewModels.Validation
{
    public class PublicationYearRangeAttribute : ValidationAttribute
    {
        private const int MinimumYear = 1;

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext)
        {
            if (value is not int publicationYear)
            {
                return ValidationResult.Success;
            }

            var calendar = new PersianCalendar();
            var currentPersianYear = calendar.GetYear(DateTime.Today);

            if (publicationYear < MinimumYear || publicationYear > currentPersianYear)
            {
                return new ValidationResult("سال انتشار وارد شده صحیح نیست");
            }

            return ValidationResult.Success;
        }
    }
}