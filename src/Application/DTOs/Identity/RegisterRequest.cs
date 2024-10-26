using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class RegisterRequest : UserInfoExResponse
{
    [Required]
    public new required string Email { get; set; }

    [Required]
    public new required string Password { get; set; }

    public override string? PhoneNumber { get; set; }

    [Required]
    public new required string UserName { get; set; }
     // Optional
}

