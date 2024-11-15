using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class AdminTopArticles :ViewComponent
    {
        private readonly NewswebsiteContext db;

        public AdminTopArticles(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var articles = db.Articles.Where(a => a.IsDeleted == false && a.Published == true).OrderByDescending(a => a.Views).Take(5).Select(a => new DisplayArticleShortenVM
            {
                Id = a.Id,
                Title = a.Title,
                Views = a.Views,
                Date = a.Date,
            });

            return View(articles);
        }
    }
}
