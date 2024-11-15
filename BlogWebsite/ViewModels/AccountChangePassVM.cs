using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class AccountChangePassVM
    {
        [Display(Name = "Mật khẩu hiện tại")]
        [Required(ErrorMessage = "Không được để trống.")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Không được để trống.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}
