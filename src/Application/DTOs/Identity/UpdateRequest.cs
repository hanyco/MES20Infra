using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class UpdateRequest
{
    [Required]
    public string DisplayName { get; set; }

    public string Email { get; set; }

    [Required]
    public string Id { get; set; }

    [Required]
    [MinLength(4)]
    public string UserName { get; set; }
    public string? PhoneNumber { get; set; }
}