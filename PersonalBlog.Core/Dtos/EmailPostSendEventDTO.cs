namespace PersonalBlog.Models.Dtos
{
    public partial class EmailPostSendEventDTO
    {
        public Guid Id { get; set; }

        public string Recipient { get; set; } = null!;

        public DateTime SentOn { get; set; }

        public Guid? IdentityUserId { get; set; }

        public Guid PostId { get; set; }

    }
}