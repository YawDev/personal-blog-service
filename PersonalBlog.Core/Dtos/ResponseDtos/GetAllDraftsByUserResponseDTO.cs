namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class GetAllDraftsByUserResponseDTO
    {
        public List<DraftDTO> UnfinishedDrafts { get; set; } = null!;
    }
}