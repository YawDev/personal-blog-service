using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Api.Contracts.Response.Blogs
{
    public class GetAllBlogsResponse
    {
        public List<BlogResponse> Blogs { get; set; } = default!;
    }
}