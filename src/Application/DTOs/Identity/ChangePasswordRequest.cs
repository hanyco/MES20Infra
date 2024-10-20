using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class ChangePasswordRequest
{
    public string UserId { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}

//public class UpdateRequest
//{
//    public string Id { get; set; }

//    [Required]
//    public string UserName { get; set; }

//    [Required]
//    [EmailAddress]
//    public string Email { get; set; }
//}
