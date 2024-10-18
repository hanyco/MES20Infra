using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class RegisterRequest
{
    public DateTime? BrithDate { get; set; }
    [Required]
    [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match")]
    public required string ConfirmPassword { get; set; }

    [Required]
    public required string DisplayName { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "The password field must contain at least 4 characters")]
    public required string Password { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    [Phone]
    public string? PhoneNumber { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "The username field must contain at least 4 characters")]
    public required string UserName { get; set; }
}
