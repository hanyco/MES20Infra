using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Security;
public interface IAccessControlService
{
    /// <summary>
    /// Check if a user has access to a specific entity with a specific access type.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="entityName">The name of the entity.</param>
    /// <param name="accessType">The type of access requested (e.g. Read, Write).</param>
    /// <returns>Returns true if the user has the requested access; otherwise false.</returns>
    Task<bool> HasAccess(string userId, string entityName, string accessType);
}