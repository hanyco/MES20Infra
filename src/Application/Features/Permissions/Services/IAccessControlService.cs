using Application.Common.Enums;
using Application.Infrastructure.Persistence;

namespace Application.Features.Permissions.Services;

public interface IAccessControlService
{
    Task<AccessLevel> GetAccessLevel(string userId, string path);
    Task<AccessLevel> GetAccessLevel(string userId, long entityId);    
}