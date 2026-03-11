using System;
using System.Collections.Generic;

namespace PersonalBlog.Models.DatabaseModels;

public partial class Draft
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Preview { get; set; } = null!;

    public DateTime? Createdon { get; set; }

    public Guid Userid { get; set; }

    public DateTime? Lastmodifieddate { get; set; }

    public virtual BlogUser User { get; set; } = null!;
}
