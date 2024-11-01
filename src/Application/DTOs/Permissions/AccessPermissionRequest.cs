namespace Application.DTOs.Permissions;

public sealed class AccessPermissionRequest
{
    public required string UserId { get; set; }
    public long EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string? AccessType { get; set; }
    public string? AccessScope { get; set; }
}