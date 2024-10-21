using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Controllers;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public NavigationViewComponent(NewswebsiteContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var categories = db.Categories.Where(c => c.IsDeleted == false).Select(c => new NavigationVM
            {
                Id = c.Id,
                Name = c.Name,
            });

            return View(categories);
        }
    }
}
