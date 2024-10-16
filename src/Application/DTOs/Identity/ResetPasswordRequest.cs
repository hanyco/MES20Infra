using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class ResetPasswordRequest
{
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(4)]
    public string Password { get; set; }

    [Required]
    public string Token { get; set; }
}
