
namespace LibraryManagement.DTO.Book
{
    public class BookListQueryDto : BookFilterDto
    {

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}
