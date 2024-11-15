using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class TagChartViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public TagChartViewComponent(NewswebsiteContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var chartData = new TagChartVM();

            chartData.ArticlesByTag = db.Tags.Include(t => t.Articles).Where(t => t.IsDeleted != true).OrderBy(t => t.Name).Select(t => t.Articles.Count).ToList();

            chartData.TagNames = db.Tags.Where(t => t.IsDeleted != true).OrderBy(t => t.Name).Select(t => t.Name).ToList();

            return View(chartData);
        }
    }
}
