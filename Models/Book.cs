namespace LibraryManagement.Models
{
    public class Book : BaseEntity
    {
        public string  Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int PublicationYear { get; set; }
        public PublicationSeason? PublicationSeason { get; set; }
        public int PageCount { get; set; }
        public int Edition {  get; set; }
        public int Volume { get; set; }

    }
    public enum PublicationSeason
    {
        spring ,
        summer ,
        autumn,
        winter
    }
}
