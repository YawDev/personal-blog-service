namespace PersonalBlog.Models.Dtos
{
    public class CreateBlogDTO
    {
        public string UserGuid { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Preview { get; set; } = null!;
    }
}