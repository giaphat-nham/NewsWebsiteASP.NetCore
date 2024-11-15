using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels
{
    public class AccountDeleteVM
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? Role { get; set; }
        public bool IsDeleted { get; set; }
    }
}
