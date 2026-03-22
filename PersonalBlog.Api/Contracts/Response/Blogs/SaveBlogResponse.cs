namespace PersonalBlog.Api.Contracts.Response.Blogs
{
    public class SaveBlogResponse : SaveBaseResponse
    {
        public Guid PostGuid { get; set; }
    }
}