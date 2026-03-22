using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class GetBlogsByUserResponseDTO
    {
        public BlogPostedByDTO User { get; set; }
        public List<PostDTO> Blogs { get; set; } = null!;
    }
}