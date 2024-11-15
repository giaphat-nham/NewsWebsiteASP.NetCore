namespace NewsWebsite.ViewModels
{
    public class TagChartVM
    {
        public List<string> TagNames { get; set; } = new List<string>();

        public List<int> ArticlesByTag { get; set; } = new List<int>();
    }
}
