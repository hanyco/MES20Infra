using Application.Common.Enums;
using Application.Infrastructure.Persistence;
using Application.Interfaces.Permissions.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Application.Features.Permissions.Repositories;
public class AccessPermissionRepository(IdentityDbContext context) : IAccessPermissionRepository
{
    private readonly IdentityDbContext _context = context;

    /// <summary>
    /// Retrieve the access permission for a specific user and entity.
    /// </summary>
    /// <param name="userId">User's ID</param>
    /// <param name="entityId">Entity's ID</param>
    /// <returns>Access permission if found; otherwise, null.</returns>
    public async Task<AccessPermission?> GetAccessPermissionAsync(string userId, long entityId)
    {
        var query = from ap in _context.AccessPermissions
                    where ap.UserId == userId && ap.EntityId == entityId
                    select ap;

        var dbResult = await query.FirstOrDefaultAsync();
        return dbResult;
    }


    /// <summary>
    /// Retrieve the parent access permission to check for recursive access.
    /// </summary>
    /// <param name="entityId">Entity's ID</param>
    /// <returns>Parent access permission if found; otherwise, null.</returns>
    // Revised GetParentPermissionAsync method
    public async Task<AccessPermission?> GetParentPermissionAsync(string userId, long entityId)
    {
        // Query to find the parent AccessPermission based on UserId and ParentId of the entity
        var query = from ap in _context.AccessPermissions
                    where ap.UserId == userId && ap.Id == entityId // Match UserId and the entity's Id
                    select ap.ParentId; // Fetch the ParentId

        // Retrieve the parent permission using ParentId if it exists
        var parentId = await query.FirstOrDefaultAsync();

        if (parentId.HasValue)
        {
            // Query the AccessPermissions again to find the actual parent permission based on ParentId
            return await _context.AccessPermissions.FirstOrDefaultAsync(ap => ap.Id == parentId.Value);
        }

        return null; // Return null if there's no parent
    }

    public async Task<AccessPermission?> GetAccessPermissionWithParentAsync(string userId, long entityId)
    {
        var query = from p in _context.AccessPermissions
                    where p.UserId == userId && (p.EntityId == entityId || p.EntityId == _context.AccessPermissions.FirstOrDefault(x => x.EntityId == entityId).ParentId)
                    orderby p.EntityId == entityId descending
                    select p;

        return await query.FirstOrDefaultAsync();
    }
}