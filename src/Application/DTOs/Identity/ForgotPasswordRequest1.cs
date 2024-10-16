﻿using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}
