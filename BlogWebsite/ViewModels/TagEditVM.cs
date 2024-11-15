using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class TagEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống tên thẻ")]
        [Display(Name = "Tên thẻ")]
        public string? Name { get; set; }
    }
}
