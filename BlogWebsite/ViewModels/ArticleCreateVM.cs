using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class ArticleCreateVM
    {
        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        public string? Title { get; set; }

        [Display(Name = "Mô tả")]
        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Vui lòng nhập nội dung bài viết")]
        public string? Htext { get; set; }

        [Display(Name = "Đã phát hành")]
        public bool Published { get; set; } = false;

        [Display(Name = "Danh mục bài viết")]
        public int CategoryId { get; set; }
    }
}
