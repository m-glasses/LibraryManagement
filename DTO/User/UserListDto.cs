namespace LibraryManagement.DTO.User
{
    public class UserListDto
    {
        public  int  Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public decimal WalletBalance { get; set; }
    }
}
