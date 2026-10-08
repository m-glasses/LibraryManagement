using LibraryManagement.Models;
using LibraryManagement.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels
{
    public class AdminUserEditViewModel
    {
        [Display(Name = "شناسه کاربر")]
        public int Id { get; set; }


        [Required(ErrorMessage = "وارد کردن نام الزامی است")]
        [StringLength(50, ErrorMessage = "نام نمی‌تواند بیشتر از ۵۰ کاراکتر باشد")]
        [Display(Name = "نام")]
        public string Name { get; set; }


        [Required(ErrorMessage = "وارد کردن نام خانوادگی الزامی است")]
        [StringLength(50, ErrorMessage = "نام خانوادگی نمی‌تواند بیشتر از ۵۰ کاراکتر باشد")]
        [Display(Name = "نام خانوادگی")]
        public string Family { get; set; }


        [Required(ErrorMessage = "وارد کردن نام پدر الزامی است")]
        [StringLength(50, ErrorMessage = "نام پدر نمی‌تواند بیشتر از ۵۰ کاراکتر باشد")]
        [Display(Name = "نام پدر")]
        public string FatherName { get; set; }


        [Required(ErrorMessage = "وارد کردن تاریخ تولد الزامی است")]
        [DataType(DataType.Date)]
        [Display(Name = "تاریخ تولد")]
        [AgeRange]
        public DateTime DateOfBirth { get; set; }


        [Required(ErrorMessage = "انتخاب میزان تحصیلات الزامی است")]
        [Display(Name = "تحصیلات")]
        public EducationLevel? Education { get; set; }


        [Required(ErrorMessage = "انتخاب جنسیت الزامی است")]
        [Display(Name = "جنسیت")]
        public Gender? Gender { get; set; }


        [StringLength(300, ErrorMessage = "آدرس نمی‌تواند بیشتر از ۳۰۰ کاراکتر باشد")]
        [Display(Name = "آدرس")]
        public string? Address { get; set; }


        [Required(ErrorMessage = "وارد کردن نام کاربری الزامی است")]
        [StringLength(50, ErrorMessage = "نام کاربری نمی‌تواند بیشتر از ۵۰ کاراکتر باشد")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "وارد کردن ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل وارد شده صحیح نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }


        [Required(ErrorMessage = "وارد کردن شماره تلفن الزامی است")]
        [Display(Name = "شماره تلفن")]
        [IranianPhoneNumber]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "انتخاب نقش کاربر الزامی است")]
        [Display(Name = "نقش")]
        public string Role { get; set; }
    }
}