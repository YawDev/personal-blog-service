namespace PersonalBlog.Core.Dtos.RequestDtos
{
    public class CreateBlogDTO
    {
        public string UserGuid { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Preview { get; set; } = null!;
    }
}