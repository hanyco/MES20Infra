using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class ChangePasswordRequest
{

    public required string Id { get; set; }


    [MinLength(4)]
    public required string Password { get; set; }


    [Compare("Password")]
    public required string ConfirmPassword { get; set; }
}
