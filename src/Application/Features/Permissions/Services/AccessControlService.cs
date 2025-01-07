using Application.Common.Enums;
using Application.Interfaces.Permissions.Repositories;

namespace Application.Features.Permissions.Services;

internal class AccessControlService(IAccessControlRepository accessPermissionRepository) : IAccessControlService
{
    private readonly IAccessControlRepository _accessPermissionRepository = accessPermissionRepository;

    public async Task<AccessLevel> GetAccessLevel(string userId, string path)
    {
        var entityId = this.GetEntityIdByPath(path);
        if (entityId is { } id)
            return await this.GetAccessLevel(userId, id);
        else 
            return AccessLevel.ReadOnly;
    }

    public async Task<AccessLevel> GetAccessLevel(string userId, long entityId)
    {
        if (userId is null)
        {
            return AccessLevel.NoAccess;
        }
        // Retrieve access permission directly and from parent in a single query if possible
        var accessPermission = await this._accessPermissionRepository.GetAccessPermissionWithParentAsync(userId, entityId);

        if (accessPermission == null)
        {
            return AccessLevel.None;
        }

        // Map AccessType to AccessLevel
        return AccessLevelHelper.MapAccessType(accessPermission.AccessType);
    }

    private long? GetEntityIdByPath(string path) =>
        // TODO: Implement a path-to-entity mapping logic
        default;
}

