using PersonalBlog.Api.Contracts.Response.Blogs;
using PersonalBlog.Core.Dtos;

namespace PersonalBlog.Api.Contracts.Response.Auth
{
    public class LoginResponse
    {
        public IdentityUserResponse User { get; set; } = default!;
    }

    public class CheckIdentityResponse
    {
        public IdentityUserResponse User { get; set; } = default!;
    }
}