namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class GetAllDraftsByUserResponseDTO
    {
        public List<DraftResponseDTO> UnfinishedDrafts { get; set; } = null!;
    }
}