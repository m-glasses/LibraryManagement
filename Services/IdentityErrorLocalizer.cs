using System.Reflection.Metadata.Ecma335;

namespace LibraryManagement.Services
{
    public class IdentityErrorLocalizer
    {
        public string Localizer(string errorCode)
        {
            return errorCode switch
            {
                "PasswordTooShort" =>
                    "رمز عبور کوتاه است",

                "PasswordRequiresDigit" =>
                    "رمز عبور باید حداقل یک عدد داشته باشد",

                "PasswordRequiresLower" =>
                    "رمز عبور باید حداقل یک حرف کوچک انگلیسی داشته باشد",

                "PasswordRequiresUpper" =>
                    "رمز عبور باید حداقل یک حرف بزرگ انگلیسی داشته باشد",

                "PasswordRequiresNonAlphanumeric" =>
                    "رمز عبور باید حداقل یک کاراکتر خاص داشته باشد",

                "DuplicateUserName" =>
                    "این نام کاربری قبلاً ثبت شده است",

                "DuplicateEmail" =>
                    "این ایمیل قبلاً ثبت شده است",

                _ =>
                    "خطایی هنگام ایجاد حساب کاربری رخ داد"
            };
        }
    }
}
