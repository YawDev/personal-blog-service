namespace PersonalBlog.Models.BusinessModels
{
    public class SmtpOptions
    {
        public string Host { get; init; } = "";
        public int Port { get; init; } = 587;
        public string Username { get; init; } = "";
        public string Password { get; init; } = "";
        public string FromEmail { get; init; } = "";
        public string FromName { get; init; } = "";
    }
}