using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents

{
    public class RecommendedArticlesViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public RecommendedArticlesViewComponent(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke(int categoryId)
        {
            var articles = db.Articles.Where(a => a.IsDeleted == false && a.Published == true && a.CategoryId == categoryId).OrderByDescending(a => a.Views).Take(2).Select(a => new RecommendedArticleVM
            {
                Id = a.Id,
                Title = a.Title,
                Thumbnail = a.Thumbnail,
                Views = a.Views
            });

            return View(articles);
        }
    }
}
