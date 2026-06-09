namespace PersonalBlog.Core.BusinessContext
{
    public class EmailSendResult
    {
        public bool Succeeded { get; init; }
        public string? ProviderResponse { get; init; }
        public string? Error { get; init; }
    }
}