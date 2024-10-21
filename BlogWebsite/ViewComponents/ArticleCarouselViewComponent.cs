using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class ArticleCarouselViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public ArticleCarouselViewComponent(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var articles = db.Articles.Where(a => a.IsDeleted == false && a.Published == true).OrderByDescending(a => a.Views).Take(6).Select(a => new HomeArticleVM
            {
                Id = a.Id,
                Title = a.Title,
                Views = a.Views,
                Date = a.Date,
                Thumbnail = a.Thumbnail,
            });

            return View(articles);
        }
    }
}
