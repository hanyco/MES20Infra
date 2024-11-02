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
    private readonly IdentityDbContext _context;

    public AccessControlService(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasAccess(string userId, string entityName, string accessType)
    {
        // Check the specific entity and its parent, recursively if needed.
        return await HasAccessRecursive(userId, entityName, accessType);
    }

    private async Task<bool> HasAccessRecursive(string userId, string entityName, string accessType)
    {
        // Find the access permission for the current entity
        var permission = await _context.AccessPermissions
            .FirstOrDefaultAsync(p => p.UserId == userId && p.EntityName == entityName && p.AccessType == accessType);

        if (permission != null)
        {
            return true;
        }

        // If there is no specific access permission, check the parent entity recursively
        var parentEntityId = await _context.AccessPermissions
            .Where(p => p.EntityName == entityName)
            .Select(p => p.ParentId)
            .FirstOrDefaultAsync();

        if (parentEntityId != 0) // ParentId is found
        {
            var parentEntityName = await _context.AccessPermissions
                .Where(p => p.Id == parentEntityId)
                .Select(p => p.EntityName)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(parentEntityName))
            {
                return await HasAccessRecursive(userId, parentEntityName, accessType);
            }
        }

        return false;
    }
}
