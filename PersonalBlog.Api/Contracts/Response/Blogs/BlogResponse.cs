namespace PersonalBlog.Api.Contracts.Response.Blogs
{
    public class BlogResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Preview { get; set; } = null!;

        public DateTime? DatePosted { get; set; }

        public string Author { get; set; } = null!;

        public Guid UserId { get; set; }
    }
}