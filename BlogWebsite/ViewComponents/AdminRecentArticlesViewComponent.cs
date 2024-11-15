using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class AdminRecentArticlesViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public AdminRecentArticlesViewComponent(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var articles = db.Articles.Include(a => a.Category).Where(a => a.IsDeleted == false).OrderByDescending(a => a.Date).Take(3).Select(a => new DisplayArticleVM
            {
                Id = a.Id,
                Title = a.Title,
                Views = a.Views,
                Date = a.Date,
                Thumbnail = a.Thumbnail,
                Category = a.Category.Name
            });

            return View(articles);
        }
    }
}
