using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class TagCreateVM
    {
        [Required(ErrorMessage = "Không được để trống tên danh mục.")]
        [Display(Name = "Tên thẻ")]
        public string? Name { get; set; }
    }
}
