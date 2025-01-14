using Application.Common.Enums;

namespace Application.Features.Permissions.Services;

public interface IAccessControlService
{
    /// <summary>
    /// Retrieves the access level of a user to a specific path.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    Task<AccessLevel> GetAccessLevel(string userId, string path);

    /// <summary>
    /// Retrieves the access level of a user to a specific entity.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    Task<AccessLevel> GetAccessLevel(string userId, long entityId);
}