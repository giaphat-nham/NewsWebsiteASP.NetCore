using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class StatisticCategoryViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public StatisticCategoryViewComponent(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var totalArticle = db.Articles.Count(a => a.IsDeleted != true);
            var categories = db.Categories.Include(c => c.Articles).Where(c => c.IsDeleted == false).Select(c => new StatisticCategoryVM
            {
                Id = c.Id,
                Name = c.Name,
                Percentage = ((float)c.Articles.Count / totalArticle) * 100F,
            });

            return View(categories);
        }
    }
}
