using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class CategoryCreateVM
    {
        [Required(ErrorMessage = "Không được để trống tên danh mục.")]
        [Display(Name = "Tên danh mục")]
        public string? Name { get; set; }
    }
}
