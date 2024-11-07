using Application.Common.Enums;
using Application.Interfaces.Permissions.Repositories;

namespace Application.Features.Permissions.Services;

public class AccessControlService(IAccessPermissionRepository accessPermissionRepository) : IAccessControlService
{
    private readonly IAccessPermissionRepository _accessPermissionRepository = accessPermissionRepository;

    public async Task<AccessLevel> GetAccessLevel(string userId, string path)
    {
        var entityId = this.GetEntityIdByPath(path);
        return await GetAccessLevel(userId, entityId);
    }

    public async Task<AccessLevel> GetAccessLevel(string userId, long entityId)
    {
        // Retrieve access permission directly and from parent in a single query if possible
        var accessPermission = await _accessPermissionRepository.GetAccessPermissionWithParent(userId, entityId);

        if (accessPermission == null)
            return AccessLevel.None;

        // Map AccessType to AccessLevel
        return AccessLevelHelper.MapAccessType(accessPermission.AccessType);
    }

    private long GetEntityIdByPath(string path)
    {
        // TODO: Implement a path-to-entity mapping logic
        throw new NotImplementedException();
    }
}

