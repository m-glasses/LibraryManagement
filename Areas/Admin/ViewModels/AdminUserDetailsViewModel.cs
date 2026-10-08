using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels
{
    public class AdminUserDetailsViewModel
    {
        [Display(Name = "شناسه کاربر")]
        public int Id { get; set; }

        [Display(Name = "نام کامل")]
        public string FullName { get; set; }

        [Display(Name = "نام پدر")]
        public string FatherName { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "جنسیت")]
        public Gender Gender { get; set; }

        [Display(Name = "تحصیلات")]
        public EducationLevel Education { get; set; }

        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }

        [Display(Name = "نقش")]
        public string Role { get; set; }

        [Display(Name = "آدرس")]
        public string? Address { get; set; }

        [Display(Name = "موجودی کیف پول")]
        public decimal WalletBalance { get; set; }

        [Display(Name = "وضعیت بدهی")]
        public bool IsDebtor { get; set; }

        [Display(Name = "وضعیت حساب")]
        public bool IsActive { get; set; }
    }
}