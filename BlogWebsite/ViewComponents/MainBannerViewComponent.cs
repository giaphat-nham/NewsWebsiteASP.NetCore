using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsWebsite.Models;

namespace NewsWebsite.ViewComponents
{
    public class MainBannerViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public MainBannerViewComponent(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var article = db.Articles.Include(a => a.Account).Where(a => a.IsDeleted == false && a.Published == true).OrderByDescending(a => a.Date).FirstOrDefault();

            return View(article);
        }
    }
}
