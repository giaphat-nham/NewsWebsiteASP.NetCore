using System;
using System.Collections.Generic;

namespace NewsWebsite.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
