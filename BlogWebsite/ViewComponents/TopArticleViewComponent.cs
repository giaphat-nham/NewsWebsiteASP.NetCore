using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace NewsWebsite.ViewComponents
{
    public class TopArticleViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public TopArticleViewComponent(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var article = db.Articles.Where(a => a.IsDeleted == false && a.Published == true).OrderByDescending(a => a.Views).FirstOrDefault();

            return View(article);
        }
    }
}
