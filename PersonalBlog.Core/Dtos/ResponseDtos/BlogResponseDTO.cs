namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class BlogResponseDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Preview { get; set; } = null!;

        public DateTime? DatePosted { get; set; }

        public Guid UserId { get; set; }
    }
}