﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class UpdateRequest
{
    public string? DisplayName { get; set; }

    public string? Email { get; set; }

    public string? UserId { get; set; }

    [Required]
    [MinLength(4)]
    public string? UserName { get; set; }

    [PasswordPropertyText]
    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }
}