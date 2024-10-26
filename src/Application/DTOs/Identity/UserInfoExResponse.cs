namespace Application.DTOs.Identity;

public class UserInfoExResponse : UserInfoResponse
{
    public virtual string? DisplayName { get; set; }
    public virtual string? Email { get; set; }
    public virtual string? Password { get; set; }
    public virtual string? PhoneNumber { get; set; }
    public virtual string? UserId { get; set; }
    public virtual string? UserName { get; set; }
}