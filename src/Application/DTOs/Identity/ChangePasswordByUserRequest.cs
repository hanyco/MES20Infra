using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class ChangePasswordByUserRequest
{
    [MinLength(4)]
    public required string OldPassword { get; set; }

    [MinLength(4)]
    public required string NewPassword { get; set; }


    [Compare(nameof(NewPassword))]
    public required string ConfirmNewPassword { get; set; }
}