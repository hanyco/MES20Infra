using System.Security.Claims;

using Application.DTOs.Permissions;
using Application.Features.Identity;
using Application.Features.Permissions.Services;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
public class AccessPermissionsController(IIdentityService identityService, IAccessControlService accessControlService) : ControllerBase
{
    private readonly IAccessControlService _accessControlService = accessControlService;
    private readonly IIdentityService _identityService = identityService;

    [HttpGet("{entityId}")]
    public async Task<IActionResult> GetAccessPermission(long entityId)
    {
        var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized("User not authenticated");
        }

        var accessLevel = await this._accessControlService.GetAccessLevel(userId, entityId);
        return this.Ok(new { EntityId = entityId, AccessLevel = accessLevel.ToString() });
    }

    [HttpDelete("{entityId}")]
    public IActionResult RemoveAccessPermission(long entityId) =>
        this.Ok($"Access to EntityId {entityId} removed.");

    [HttpPost]
    public async Task<IActionResult> SetAccessPermissions([FromBody] AccessPermissionRequest request)
    {
        var result = await this._identityService.SetAccessPermissions(request);
        return !result.IsSucceed ? this.BadRequest(result.Message) : this.Ok(result.Message);
    }
}
