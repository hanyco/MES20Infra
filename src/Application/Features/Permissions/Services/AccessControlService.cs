using Application.Common.Enums;
using Application.Interfaces.Permissions.Repositories;

namespace Application.Features.Permissions.Services;

public class AccessControlService : IAccessControlService
{
    private readonly IAccessPermissionRepository _accessPermissionRepository;

    public AccessControlService(IAccessPermissionRepository accessPermissionRepository) => this._accessPermissionRepository = accessPermissionRepository;

    public async Task<AccessLevel> GetAccessLevel(string userId, long entityId)
    {
        // Step 1: Find user's access permission for the given entity
        var accessPermission = await _accessPermissionRepository.GetAccessPermissionAsync(userId, entityId);

        // If no direct access permission found, check the parent entity's permissions
        if (accessPermission == null)
        {
            var parentPermission = await _accessPermissionRepository.GetParentPermissionAsync(userId, entityId);

            // If a parent entity exists, call HasAccessRecursiveAsync for the parent entity
            if (parentPermission != null)
            {
                var parentAccessLevel = await GetAccessLevel(userId, parentPermission.EntityId);

                // Return the parent's access level if found
                return parentAccessLevel;
            }

            // If no permission found for both the entity and parent, return NoAccess
            return AccessLevel.NoAccess;
        }

        // Return the access level for the current entity
        return Enum.TryParse<AccessLevel>(accessPermission.AccessType, out var accessLevel)
            ? accessLevel
            : AccessLevel.NoAccess;
    }
}
