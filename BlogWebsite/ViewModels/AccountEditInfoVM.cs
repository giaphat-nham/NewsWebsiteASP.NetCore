using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class AccountEditInfoVM
    {
        [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "Tên hiển thị")]
        public string? DisplayName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        public string? Username { get; set; }

        public string? Role { get; set; }
    }
}
