namespace LibraryManagement.Helper
{
    public class ErrorMessageHelper
    {
        public static string Translate(string message)
        {
            return message switch
            {
                // Users
                "User not found" or
                "User not found." =>
                    "کاربر پیدا نشد.",

                "User is already inactive." =>
                    "این کاربر در حال حاضر غیرفعال است.",

                "User is already active." =>
                    "این کاربر در حال حاضر فعال است.",

                "Invalid role." =>
                    "نقش انتخاب‌شده معتبر نیست.",

                "Education is required." =>
                    "وارد کردن میزان تحصیلات الزامی است.",

                "Gender is required." =>
                    "انتخاب جنسیت الزامی است.",

                "You cannot deactivate an admin." =>
                    "امکان غیرفعال کردن حساب مدیر وجود ندارد.",

                "First close all open loans." =>
                    "ابتدا تمام امانت‌های باز این کاربر را ببندید.",

                "First close all active reservations." =>
                    "ابتدا تمام رزروهای فعال این کاربر را ببندید.",

                "Admin users cannot be edited." =>
                    "اطلاعات مدیر قابل ویرایش نیست.",

                "Admin cannot change their role to User." =>
                    "مدیر نمی‌تواند نقش خود را به کاربر تغییر دهد.",

                // Login
                "User is inactive." =>
                    "حساب کاربری شما غیرفعال است؛ برای پیگیری با مسئول کتابخانه تماس بگیرید.",

                "UserName or Password invalid." =>
                    "نام کاربری یا رمزعبور اشتباه است.",

                "Account is locked out." =>
                    "حساب کاربری شما موقتاً قفل شده است؛ لطفاً بعداً تلاش کنید.",

                "Sign-in is not allowed." =>
                    "در حال حاضر امکان ورود به حساب کاربری وجود ندارد.",

                // Books
                "Book not found" or
                "Book not found." =>
                    "کتاب پیدا نشد.",

                "The book is currently available" =>
                    "این کتاب در حال حاضر در دسترس است و نیازی به رزرو ندارد.",

                "Book not available" =>
                    "این کتاب در حال حاضر قابل امانت نیست.",

                // Reservations
                "You already have this book on loan" =>
                    "این کتاب در حال حاضر در امانت شماست.",

                "You already have an active reservation for this book" =>
                    "شما در حال حاضر یک رزرو فعال برای این کتاب دارید.",

                "Another user has priority to borrow this book" =>
                    "کاربر دیگری برای دریافت این کتاب در اولویت است.",

                "Reservation capacity is full" =>
                    "ظرفیت رزرو این کتاب تکمیل شده است.",

                "Active reservation not found." =>
                    "رزرو فعال پیدا نشد.",

                // Library Settings
                "Library settings not found" =>
                    "تنظیمات کتابخانه پیدا نشد.",

                // Loans
                "Loan not found" =>
                    "امانت پیدا نشد.",

                "The loan is not active" =>
                    "این امانت فعال نیست.",

                "The loan cannot be returned" =>
                    "امکان بازگرداندن این امانت وجود ندارد.",

                "Invalid return date" =>
                    "تاریخ بازگشت واردشده معتبر نیست.",

                "Maximum renewal limit has been reached" =>
                    "حداکثر تعداد تمدید مجاز برای این امانت انجام شده است.",

                // Wallet
                "Wallet not found." =>
                    "کیف پول پیدا نشد.",

                "Amount must be greater than zero." =>
                    "مبلغ باید بیشتر از صفر باشد.",

                "User already has a wallet." =>
                    "این کاربر در حال حاضر کیف پول دارد.",

                "User cannot borrow while wallet balance is negative." =>
                    "به دلیل منفی بودن موجودی کیف پول، امکان دریافت امانت جدید وجود ندارد.",

                // Fallback
                _ =>
                    "خطایی رخ داده است."
            };
        }
    }
}
