using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class LoginVM
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Không được để trống.")]
        public string Username { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Không được để trống.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
