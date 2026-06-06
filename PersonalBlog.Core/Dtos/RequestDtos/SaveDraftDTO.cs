using PersonalBlog.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlog.Core.Dtos.RequestDtos
{
    public class SaveDraftDTO
    {
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Preview { get; set; } = null!;

    }

    public class SendEmailDTO
    {
        public Guid PostId { get; set; }

        public string RecipientEmail { get; set; } = null!;

        public Guid? IdentityUserId { get; set; }

        public string BlogShareLink { get; set; } = null!;
    }
}
