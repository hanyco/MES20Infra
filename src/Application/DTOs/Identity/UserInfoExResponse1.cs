namespace Application.DTOs.Identity;

public sealed class UserInfoExResponse : UserInfoResponse
{
    public string DisplayName { get; internal set; }
    public string UserId { get; internal set; }
    public string? UserName { get; internal set; }
    public string? Email { get; internal set; }
    public string? PhoneNumber { get; internal set; }
}
