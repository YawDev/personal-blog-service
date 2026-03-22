using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class GetAllBlogsResponseDTO
    {
        public List<BlogResponseDTO> Blogs { get; set; } = null!;
    }
}