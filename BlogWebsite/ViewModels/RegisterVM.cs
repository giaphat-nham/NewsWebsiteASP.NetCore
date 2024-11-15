using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class RegisterVM
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Không được để trống tên đăng nhập.")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Không được để trống mật khẩu.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Tên hiển thị")]
        [Required(ErrorMessage = "Không được để trống tên hiển thị.")]
        public string DisplayName { get; set; }

        [Display(Name = "Vai trò")]
        public int RoleId { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Không được để trống email.")]
        [EmailAddress(ErrorMessage = "Sai định dạng Email")]
        public string Email { get; set; }
    }
}
