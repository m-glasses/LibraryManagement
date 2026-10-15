using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Areas.Admin.ViewModels.User
{
    public class AdminUserIndexViewModel
    {
        public List<UserListViewModel> Users { get; set; } = new();

        public AdminUserFilterViewModel? Filter { get; set; } 

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}