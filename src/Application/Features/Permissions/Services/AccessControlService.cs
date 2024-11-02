using Application.Infrastructure.Persistence;
using Application.Interfaces.Security;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Permissions.Services;
public class AccessControlService : IAccessControlService
{
    private readonly IdentityDbContext _dbContext;

    public AccessControlService(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasAccess(string userId, string path)
    {
        // بررسی مسیر (این متد می‌تواند در آینده تغییر کند تا دقت بیشتری پیدا کند)
        // مثلاً می‌توانید path را به موجودیت و نوع عملیات مربوطه نگاشت کنید
        var accessPermission = await _dbContext.AccessPermissions
            .FirstOrDefaultAsync(ap => ap.UserId == userId && ap.EntityType == "Path" && ap.EntityId == path.GetHashCode());

        return accessPermission != null && accessPermission.AccessType == "Read";
    }

    public async Task<bool> HasAccessToEntity(string userId, string entityType, long entityId, string accessType)
    {
        // بررسی دسترسی کاربر به یک موجودیت خاص با استفاده از AccessPermissions
        var accessPermission = await _dbContext.AccessPermissions
            .FirstOrDefaultAsync(ap => ap.UserId == userId && ap.EntityType == entityType && ap.EntityId == entityId);

        if (accessPermission == null) return false;

        // بررسی نوع دسترسی (مثلاً Read/Write)
        return accessPermission.AccessType.Equals(accessType, StringComparison.OrdinalIgnoreCase);
    }
}
