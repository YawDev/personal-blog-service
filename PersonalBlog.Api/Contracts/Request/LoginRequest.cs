using System.Text.Json.Serialization;

namespace PersonalBlog.Api.Contracts.Request
{
    public class LoginRequest
    {
            [JsonPropertyName("userName")]
        public string UserName { get; set; }
            [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}