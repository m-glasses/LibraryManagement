using LibraryManagement.Areas.Admin.ViewModels.BookCopies;
using System.ComponentModel.DataAnnotations;

public class AdminBookCopiesListViewModel
{
    public int Id { get; set; }

    [Display(Name = "شناسه کتاب")]
    public int BookId { get; set; }

    [Display(Name = "عنوان کتاب")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "نویسنده")]
    public string Author { get; set; } = string.Empty;

    [Display(Name = "ناشر")]
    public string Publisher { get; set; } = string.Empty;

    [Display(Name = "شماره اموال")]
    public string InventoryNumber { get; set; } = string.Empty;

    [Display(Name = "وضعیت نسخه")]
    public BookCopyStatus? Status { get; set; }

    [Display(Name = "آخرین امانت‌گیرنده")]
    public string? LastBorrowerName { get; set; }

    [Display(Name = "تاریخ امانت")]
    public DateTime? LastLoanStartDate { get; set; }

    [Display(Name = "موعد بازگشت")]
    public DateTime? LastLoanDueDate { get; set; }
}