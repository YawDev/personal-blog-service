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
    
    public class EditAccountRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

}