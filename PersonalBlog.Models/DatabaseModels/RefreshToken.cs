namespace PersonalBlog.Models.DatabaseModels
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } 
        public Guid IdentityUserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsExpired && !IsRevoked && !IsUsed;

        public ApplicationUser? IdentityUser { get; set; }
    }
}
