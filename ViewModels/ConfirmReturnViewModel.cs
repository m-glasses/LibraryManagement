using System.ComponentModel.DataAnnotations;

public class ConfirmReturnViewModel
{
    public int LoanId { get; set; }

    [Display(Name = "تاریخ بازگشت")]
    public DateTime ReturnDate { get; set; }
}