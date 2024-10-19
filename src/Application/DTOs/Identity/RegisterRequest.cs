using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class RegisterRequest
{
    [Required]
    public required string UserName { get; set; }
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
    public string? PhoneNumber { get; set; } // Optional
    public string? Role { get; set; } // Optional
}

