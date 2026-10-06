using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels
{
    public class UserListViewModel
    {
        [Display(Name = "شناسه کاربر")]
        public int Id { get; set; }

        [Display(Name = "نام کامل")]
        public string FullName { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }

        [Display(Name = "نقش")]
        public string Role { get; set; }
        public string RoleDisplayName => Role == "Admin" ? "مدیر" : "کاربر";

        [Display(Name = "بدهکار")]
        public bool IsDebtor { get; set; }

        [Display(Name = "موجودی کیف‌پول")]
        public decimal WalletBalance { get; set; }
    }
}