using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class FooterArticles : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public FooterArticles(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var articles = db.Articles.Where(a => a.IsDeleted == false && a.Published == true).OrderByDescending(a => a.Date).Take(2).Select(a => new ArticleSidebarVM
            {
                Id = a.Id,
                Title = a.Title,
                Date = a.Date,
                CategoryName = a.Category.Name,
                Image = a.Thumbnail,
            });

            return View(articles);
        }
    }
}
