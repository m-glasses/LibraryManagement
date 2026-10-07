namespace LibraryManagement.DTO
{
    public class UserListQueryDto
    {
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public bool? IsDebtor { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}