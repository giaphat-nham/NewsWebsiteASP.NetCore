namespace NewsWebsite.ViewModels
{
    public class ArticleDeleteVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public string? Author { get; set; }
        public int Views { get; set; }
        public bool Published { get; set; }
    }
}
