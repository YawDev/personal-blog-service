namespace PersonalBlog.Api.Contracts.Request
{

    public class CreateBlogRequest
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Preview { get; set; } = null!;
    }
}