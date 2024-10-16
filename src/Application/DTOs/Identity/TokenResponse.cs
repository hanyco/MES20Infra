using System.Text.Json.Serialization;

namespace Application.DTOs.Identity;

public sealed class TokenResponse
{
    public string Email { get; set; }
    public DateTime ExpiresOn { get; set; }
    public string Id { get; set; }
    public DateTime IssuedOn { get; set; }
    public bool IsVerified { get; set; }
    public string JWToken { get; set; }

    [JsonIgnore]
    public string RefreshToken { get; set; }

    public List<string> Roles { get; set; }
    public string UserName { get; set; }
}