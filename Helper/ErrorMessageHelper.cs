namespace LibraryManagement.Helper
{
    public class ErrorMessageHelper
    {
        public static string Translate(string message)
        {
            return message switch
            {
                "User not found" => "کاربر پیدا نشد",
                "Book not found" => "کتاب پیدا نشد",
                "The book is currently available" => "این کتاب در حال حاضر موجود است",
                "You already have this book on loan" => "این کتاب در حال حاضر در امانت شماست",
                "You already have an active reservation for this book" =>
                    "شما برای این کتاب یک رزرو فعال دارید",
                "Library settings not found" => "تنظیمات کتابخانه پیدا نشد",
                _ => "خطایی رخ داده است"
            };
        }
    }
}
