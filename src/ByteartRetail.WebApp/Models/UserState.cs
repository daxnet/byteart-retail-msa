using System.Text.Json.Serialization;

namespace ByteartRetail.WebApp.Models;

public class UserState
{
    [JsonPropertyName("id")]
    public Guid UserId { get; set; }
    
    [JsonPropertyName("name")]
    public string UserName { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }

    public override string ToString() => UserName;
}