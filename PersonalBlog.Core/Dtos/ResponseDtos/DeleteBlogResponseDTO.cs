namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class DeleteBlogResponseDTO : DeleteBaseDTO
    {
        public Guid PostGuid { get; set; }
    }
}