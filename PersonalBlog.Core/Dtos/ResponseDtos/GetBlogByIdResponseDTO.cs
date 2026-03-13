using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class GetBlogByIdResponseDTO
    {
        public PostDTO? Blog { get; set; }
    }
}