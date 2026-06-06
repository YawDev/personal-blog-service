namespace PersonalBlog.Core.Dtos.ResponseDtos
{
    public class SaveDraftResponseDTO : SaveBaseResponseDTO
    {
        public bool IsSaved { get; set; }
        public Guid DraftGuid { get; set; }
    }
        

    public class SendEmailResponseDTO 
    {
        public bool IsTriggered { get; set; }
        public Guid? EventGuid { get; set; }
    }
}