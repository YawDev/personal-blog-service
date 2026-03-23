using PersonalBlog.Api.Contracts.Response.Blogs;

namespace PersonalBlog.Api.Contracts.Response.Auth
{
    public class LoginResponse
    {
        public IdentityUserResponse User { get; set; } = default!;
    }
}