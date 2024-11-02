using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Security;
public interface IAccessControlService
{
    Task<bool> HasAccess(string userId, string path);
    Task<bool> HasAccessToEntity(string userId, string entityType, long entityId, string accessType);
}
