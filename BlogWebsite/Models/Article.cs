using System;
using System.Collections.Generic;

namespace NewsWebsite.Models;

public partial class Article
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime Date { get; set; }

    public string Description { get; set; } = null!;

    public string Htext { get; set; } = null!;

    public int Views { get; set; }

    public string Thumbnail { get; set; } = null!;

    public bool Published { get; set; }

    public int AccountId { get; set; }

    public int CategoryId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
