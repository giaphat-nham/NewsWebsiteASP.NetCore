using NewsWebsite.Models;

namespace NewsWebsite.ViewModels
{
    public class ChartVM
    {
        public List<int> ArticlesByCategory { get; set; } = new List<int>();
        public List<int> ViewsByCategory { get; set; } = new List<int>();
        public List<string> CategoriesName { get; set; } = new List<string>();
    } 
}
