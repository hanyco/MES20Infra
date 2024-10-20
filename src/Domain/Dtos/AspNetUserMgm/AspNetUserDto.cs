using System;

namespace Mes.Infra.Security;
public sealed class AspNetUserDto
{
    public String Id { get; set; }
    public String? UserName { get; set; }
    public String? NormalizedUserName { get; set; }
    public String? Email { get; set; }
    public String? NormalizedEmail { get; set; }
    public Boolean EmailConfirmed { get; set; }
    public String? PasswordHash { get; set; }
    public String? SecurityStamp { get; set; }
    public String? ConcurrencyStamp { get; set; }
    public String? PhoneNumber { get; set; }
    public Boolean PhoneNumberConfirmed { get; set; }
    public Boolean TwoFactorEnabled { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public Boolean LockoutEnabled { get; set; }
    public Int32 AccessFailedCount { get; set; }
    public String? DisplayName { get; set; }
}