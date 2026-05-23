namespace PersonalBlog.Api.Contracts.Response.Blogs
{
    public class EditAccountResponse
    {
        public bool IsUpdated { get; set; }
        public Guid UserGuid { get; set; }
    }
}
