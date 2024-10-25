namespace Application.DTOs.Identity;

public class UserInfoExResponse : UserInfoResponse
{
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public virtual string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserId { get; set; }
    public virtual string? UserName { get; set; }
}