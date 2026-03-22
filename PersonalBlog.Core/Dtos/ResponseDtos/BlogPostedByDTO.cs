namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class BlogPostedByDTO
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? DisplayName { get; set; }
    }
}