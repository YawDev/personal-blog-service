namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class DeleteDraftResponseDTO : DeleteBaseDTO
    {
        public Guid DraftGuid { get; set; }
    }
}