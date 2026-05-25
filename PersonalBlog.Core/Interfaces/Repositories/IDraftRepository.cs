using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.Interfaces.Repositories
{
    public interface IDraftRepository
    {
        Task<(Guid draftId, int result)> CreateAsync(Draft draft);
        Task<bool> DeleteAsync(Guid draftId);
        Task<bool> ExistsAsync(Guid draftId);
        Task<DraftDTO?> GetByIdAsync(Guid draftId);
        Task<List<DraftDTO>> GetByUserIdAsync(Guid userId);
        Task<int> UpdateAsync(Guid draftId, Guid userId, SaveDraftDTO draftDto);
    }
}