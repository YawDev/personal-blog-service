namespace PersonalBlog.Api.Contracts.Response.Blogs
{
    public class IdentityUserResponse
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}