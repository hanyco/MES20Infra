namespace Application.DTOs.Identity;

public class RefreshToken
{
    public DateTime Created { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime Expires { get; set; }
    public int Id { get; set; }
    public bool IsActive => this.Revoked == null && !this.IsExpired;
    public bool IsExpired => DateTime.UtcNow >= this.Expires;
    public string ReplacedByToken { get; set; }
    public DateTime? Revoked { get; set; }
    public string RevokedByIp { get; set; }
    public string Token { get; set; }
}
