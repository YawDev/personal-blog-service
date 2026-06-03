using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Interfaces.Repositories
{
    public interface IEmailPostSendEventRepository
    {
        Task<int> CreateAsync(EmailPostSendEvent emailPostSendEvent);
        Task<IEnumerable<EmailPostSendEventDTO>> GetByPostIdAsync(Guid postId);
    }
}