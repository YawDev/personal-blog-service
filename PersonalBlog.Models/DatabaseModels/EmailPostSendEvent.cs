using System;
using System.Collections.Generic;

namespace PersonalBlog.Models.DatabaseModels;
public partial class EmailPostSendEvent
{
    public Guid Id { get; set; }

    public string Recipient { get; set; } = null!;

    public DateTime SentOn { get; set; }

    public Guid? IdentityUserId { get; set; }

    public Guid PostId { get; set; }

    public virtual Post Post { get; set; } = null!;
}
