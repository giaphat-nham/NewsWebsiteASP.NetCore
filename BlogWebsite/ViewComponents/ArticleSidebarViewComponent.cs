using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class ArticleSidebar : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public ArticleSidebar(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var articles = db.Articles.Where(a => a.IsDeleted == false && a.Published == true).OrderByDescending(a => a.Views).Take(3).Select(a => new ArticleSidebarVM
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
