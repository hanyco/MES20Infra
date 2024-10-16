using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class ChangePasswordRequest
{
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    public string Id { get; set; }

    [MinLength(4)]
    public string Password { get; set; }
}
