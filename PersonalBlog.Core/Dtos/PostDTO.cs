using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Models.Dtos
{
    public class PostDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Preview { get; set; } = null!;

        public DateTime? Dateposted { get; set; }

        public Guid Userid { get; set; }

        public DateTime? Createddate { get; set; }

        public DateTime? Lastmodifieddate { get; set; }

        public virtual BlogUser User { get; set; } = null!;
    }
}