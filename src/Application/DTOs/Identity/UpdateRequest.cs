using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class UpdateRequest: UserInfoExResponse
{
    [Required]
    [MinLength(4)]
    public override string? UserName { get; set; }

    [PasswordPropertyText]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public override string? Password { get; set; }
}
