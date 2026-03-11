using System;
using Microsoft.AspNetCore.Identity;

namespace PersonalBlog.Models.DatabaseModels;

public class ApplicationUser : IdentityUser<Guid>
{
    // You can add custom properties here if needed
}