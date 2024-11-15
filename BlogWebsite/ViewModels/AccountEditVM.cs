using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class AccountEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống tên tài khoản")]
        [Display(Name = "Tên đăng nhập")]
        public string? Username { get; set; }

        public string? Password { get; set; }

        [Required(ErrorMessage = "Không được để trống tên hiển thị")]
        [Display(Name = "Tên hiển thị")]
        public string? DisplayName { get; set; }

        [Display(Name = "Vai trò")]
        public int RoleId { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        public string? RandomKey { get; set; }

        public bool IsDeleted { get; set; }
    }
}
