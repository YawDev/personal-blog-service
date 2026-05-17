namespace PersonalBlog.Api.Contracts.Response.Blogs
{
    public class DeleteBlogResponse
    {
        public bool IsDeleted { get; set; }
        public Guid PostGuid { get; set; }
    }
}
