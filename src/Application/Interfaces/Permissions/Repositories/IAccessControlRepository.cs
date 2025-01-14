using Application.Infrastructure.Persistence;

namespace Application.Interfaces.Permissions.Repositories;

public interface IAccessControlRepository
{
    /// <summary>
    /// Get the access permission for a specific user and entity.
    /// </summary>
    /// <param name="userId">ID of the user.</param>
    /// <param name="entityId">ID of the entity.</param>
    /// <returns>The access permission if found; otherwise, null.</returns>
    Task<AccessPermission?> GetAccessPermissionAsync(string userId, long entityId);

    /// <summary>
    /// Get the parent access permission for a specific entity to check recursive access.
    /// </summary>
    /// <param name="entityId">ID of the entity.</param>
    /// <returns>The access permission for the parent entity, if it exists; otherwise, null.</returns>
    Task<AccessPermission?> GetParentPermissionAsync(string userId, long entityId);

    /// <summary>
    /// Get the access permission for a specific user and entity, including the parent entity.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    Task<AccessPermission?> GetAccessPermissionWithParentAsync(string userId, long entityId);
}
