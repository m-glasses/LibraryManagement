using System.Globalization;

namespace LibraryManagement.Extensions
{
    public static class PersianDateExtensions
    {
        public static string ToPersianDate(this DateTime date)
        {
            var calendar = new PersianCalendar();

            return $"{calendar.GetYear(date):0000}/{calendar.GetMonth(date):00}/{calendar.GetDayOfMonth(date):00}";
        }
    }
}
