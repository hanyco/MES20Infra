using Application.Common.Enums;

namespace Application.Features.Permissions.Services;

public interface IAccessControlService
{
    Task<AccessLevel> GetAccessLevel(string userId, string path);
    Task<AccessLevel> GetAccessLevel(string userId, long entityId);
}