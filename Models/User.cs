namespace LibraryManagement.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string  Family { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FatherName { get; set; }
        public string Education { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public UserRole Role { get; set; }

        


    }
    public enum UserRole
    {
        User ,
        Admin
    }
    public enum Gender
    {
        Male , 
        Female
    }
}
