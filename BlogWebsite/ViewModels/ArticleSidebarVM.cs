namespace NewsWebsite.ViewModels
{
    public class ArticleSidebarVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }

        public string? CategoryName { get; set; }
        public string? Image { get; set; }
    }
}
