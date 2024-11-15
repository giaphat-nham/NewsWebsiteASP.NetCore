using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class ChartViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public ChartViewComponent(NewswebsiteContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var chartData = new ChartVM();

            chartData.ArticlesByCategory = db.Categories.Include(c => c.Articles).Where(c => c.IsDeleted != true).OrderBy(c => c.Name).Select(c => c.Articles.Count).ToList();

            var categories = db.Categories.Where(c => c.IsDeleted != true).OrderBy(c => c.Name).ToList();

            foreach (var category in categories) {
                chartData.ViewsByCategory.Add(db.Articles
                       .Where(a => a.CategoryId == category.Id && a.IsDeleted != true)
                       .Sum(a => a.Views));

                chartData.CategoriesName.Add(category.Name);
            }

            return View(chartData);
        }
    }
}
