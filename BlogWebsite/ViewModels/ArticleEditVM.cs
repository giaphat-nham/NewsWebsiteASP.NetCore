using NewsWebsite.Models;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class ArticleEditVM
    {
        public int Id { get; set; }
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

        public DateTime Date { get; set; }

        public int AccountId { get; set; }

        public string? Thumbnail { get; set; }

        public int Views { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
