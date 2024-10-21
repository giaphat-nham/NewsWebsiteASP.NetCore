namespace NewsWebsite.ViewModels
{
    public class HomeArticleVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Views { get; set; }
        public DateTime Date { get; set; }
        public string? Thumbnail { get; set; }
    }
}
