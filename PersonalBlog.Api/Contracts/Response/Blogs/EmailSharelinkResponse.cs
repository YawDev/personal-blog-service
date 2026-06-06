namespace PersonalBlog.Api.Contracts.Response.Blogs
{
    public class EmailSharelinkResponse
    {
        public bool IsTriggered { get; set; }
        public Guid? EventGuid { get; set; }
    }
}
