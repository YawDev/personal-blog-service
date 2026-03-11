using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using PersonalBlog.Models.Enums;

namespace PersonalBlog.Models.DatabaseModels;

public partial class BlogUser 
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Displayname { get; set; }

    public string? Avatar { get; set; }

    public UserRole Role { get; set; }

    public DateTime? Createddate { get; set; }

    public DateTime? Lastmodifieddate { get; set; }

    public virtual ICollection<Draft> Drafts { get; set; } = new List<Draft>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public Guid IdentityUserId { get; set; }
    public ApplicationUser IdentityUser { get; set; }
}
