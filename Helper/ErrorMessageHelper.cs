namespace LibraryManagement.Helper
{
    public class ErrorMessageHelper
    {
        public static string Translate(string message)
        {
            return message switch
            {
                "User not found" =>
                    "کاربر پیدا نشد",

                "Book not found" =>
                    "کتاب پیدا نشد",

                "The book is currently available" =>
                    "شما در حال حاضر این کتاب را در امانت دارید.",

                "You already have this book on loan" =>
                    "این کتاب در حال حاضر در امانت شماست",

                "You already have an active reservation for this book" =>
                    "شما برای این کتاب یک رزرو فعال دارید",

                "Library settings not found" =>
                    "تنظیمات کتابخانه پیدا نشد",

                "Loan not found" =>
                    "امانت پیدا نشد",

                "The loan is not active" =>
                    "این امانت فعال نیست",

                "The loan cannot be returned" =>
                    "امکان بازگشت این امانت وجود ندارد",

                "Invalid return date" =>
                    "تاریخ بازگشت واردشده معتبر نیست",

                "Maximum renewal limit has been reached" =>
                    "حداکثر تعداد تمدید مجاز انجام شده است",

                _ =>
                    "خطایی رخ داده است"
            };
        }
    }
}