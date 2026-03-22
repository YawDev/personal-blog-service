namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class SaveDraftResponseDTO : SaveBaseResponseDTO
        {
            public Guid DraftGuid { get; set; }
        }
}