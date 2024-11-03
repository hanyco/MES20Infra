using Application.Common.Enums;

namespace Application.Interfaces.Security;

public interface IAccessControlService
{
    // Checks access level for a specific entity and user based on required access level
    Task<AccessLevel> HasAccessAsync(string userId, Guid entityId, AccessLevel requiredAccess);

    // Recursive check for access on the entity hierarchy
    Task<AccessLevel> HasAccessRecursiveAsync(string userId, Guid entityId, AccessLevel requiredAccess);
}
