using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Identity;
public class UserInfoResponse;

public sealed class UserInfoExResponse : UserInfoResponse
{
    public string DisplayName { get; internal set; }
    public string UserId { get; internal set; }
    public string? UserName { get; internal set; }
    public string? Email { get; internal set; }
    public string? PhoneNumber { get; internal set; }
}

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}

public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Token { get; set; }

    [Required]
    [MinLength(4)]
    public required string Password { get; set; }

    [Required]
    [Compare("Password")]
    public required string ConfirmPassword { get; set; }
}

public class ChangePasswordRequest
{

    public required string Id { get; set; }


    [MinLength(4)]
    public required string Password { get; set; }


    [Compare("Password")]
    public required string ConfirmPassword { get; set; }
}

public class ChangePasswordByUserRequest
{
    [MinLength(4)]
    public required string OldPassword { get; set; }

    [MinLength(4)]
    public required string NewPassword { get; set; }


    [Compare(nameof(NewPassword))]
    public required string ConfirmNewPassword { get; set; }
}