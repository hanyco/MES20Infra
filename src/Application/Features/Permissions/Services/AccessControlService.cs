using System;
using System.Threading.Tasks;

using Application.Common.Enums;
using Application.Interfaces.Security;

using Microsoft.Extensions.Logging;

namespace Application.Infrastructure.Services
{
    public class AccessControlService : IAccessControlService
    {
        private readonly IAccessPermissionRepository _accessPermissionRepository;
        private readonly ILogger<AccessControlService> _logger;

        public AccessControlService(IAccessPermissionRepository accessPermissionRepository, ILogger<AccessControlService> logger)
        {
            _accessPermissionRepository = accessPermissionRepository;
            _logger = logger;
        }

        public async Task<AccessLevel> HasAccessAsync(string userId, Guid entityId, AccessLevel requiredAccess)
        {
            // شروع چک کردن سطح دسترسی با فراخوانی متد بازگشتی
            return await HasAccessRecursiveAsync(userId, entityId, requiredAccess);
        }

        public async Task<AccessLevel> HasAccessRecursiveAsync(string userId, Guid entityId, AccessLevel requiredAccess)
        {
            // دریافت دسترسی کاربر به این entity
            var accessPermission = await _accessPermissionRepository.GetAccessPermissionAsync(userId, entityId);
            if (accessPermission == null)
            {
                return AccessLevel.NoAccess;
            }

            // اگر دسترسی دقیقاً منطبق با درخواست باشد، برگردانید
            if (accessPermission.AccessLevel == AccessLevel.NoAccess)
            {
                return AccessLevel.NoAccess;
            }
            else if (accessPermission.AccessLevel == AccessLevel.ReadOnly && requiredAccess == AccessLevel.FullAccess)
            {
                return AccessLevel.NoAccess;
            }

            // در صورت امکان، بالاترین سطح دسترسی را برگردانید
            return accessPermission.AccessLevel;
        }
    }
}
