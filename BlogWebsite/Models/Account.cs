using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public string DisplayName { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public string Email { get; set; } = null!;

    public string RandomKey { get; set; } = null!;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual Role Role { get; set; } = null!;
}
