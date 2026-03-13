using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class GetAllBlogsResponseDTO
    {
        public List<PostDTO> Blogs { get; set; } = null!;
    }
}