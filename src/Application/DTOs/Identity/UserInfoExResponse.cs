namespace Application.DTOs.Identity;

public sealed class UserInfoExResponse : UserInfoResponse
{
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }
}