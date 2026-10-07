namespace LibraryManagement.DTO
{
    public class UserListResult
    {
        public List<UserListDto> Users { get; set; } = new();

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages =>
            PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}