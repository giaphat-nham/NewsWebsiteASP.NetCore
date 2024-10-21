using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Models;
using NewsWebsite.ViewModels;

namespace NewsWebsite.ViewComponents
{
    public class CategoryListViewComponent : ViewComponent
    {
        private readonly NewswebsiteContext db;

        public CategoryListViewComponent(NewswebsiteContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var categories = db.Categories.Where(c => c.IsDeleted == false).Select(c => new CategoryListVM
            {
                Id = c.Id,
                Name = c.Name
            });

            return View("List",categories);
        }
    }
}
