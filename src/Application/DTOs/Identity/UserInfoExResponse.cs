﻿namespace Application.DTOs.Identity;

public sealed class UserInfoExResponse : UserInfoResponse
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid? UserGroupId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string DisplayName { get; internal set; }
}
