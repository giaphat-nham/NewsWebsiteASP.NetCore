using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class CategoryEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống tên danh mục")]
        [Display(Name = "Tên danh mục")]
        public string? Name { get; set; }

    }
}
