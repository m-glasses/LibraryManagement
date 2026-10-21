namespace LibraryManagement.DTO.User
{
    public class UserFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public bool? IsDebtor { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? SortBy {  get; set; }
        public bool SortDescending { get; set; } = false;
    }
}