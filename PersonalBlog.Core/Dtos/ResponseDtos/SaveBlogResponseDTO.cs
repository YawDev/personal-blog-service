namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class SaveBlogResponseDTO : SaveBaseResponseDTO
    {
        public Guid PostGuid { get; set; }
    }
}