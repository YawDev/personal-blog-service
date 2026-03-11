using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;
using PersonalBlog.Models.Enums;

namespace PersonalBlog.Core.Dtos
{
    public class BlogUserDTO
    {
         public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Displayname { get; set; }

        public string? Avatar { get; set; }

        public UserRole Role { get; set; }

        public DateTime? Createddate { get; set; }

        public DateTime? Lastmodifieddate { get; set; }

        public List<DraftDTO> Drafts { get; set; } = new List<DraftDTO>();

        public List<PostDTO> Posts { get; set; } = new List<PostDTO>();

        public Guid IdentityUserId { get; set; }
        public ApplicationUser IdentityUser { get; set; }
    }
}