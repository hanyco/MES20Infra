using Application.DTOs.Permissions;
using Application.Features.Identity;
using Application.Features.Permissions.Services;

using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not authenticated");

        var accessLevel = await _accessControlService.GetAccessLevel(userId, entityId);
        return Ok(new { EntityId = entityId, AccessLevel = accessLevel.ToString() });
    }

    [HttpPost]
    public async Task<IActionResult> SetAccessPermissions([FromBody] AccessPermissionRequest request)
    {
        var result = await _identityService.SetAccessPermissions(request);
        if (!result.IsSucceed)
            return BadRequest(result.Message);

        return Ok(result.Message);
    }

    [HttpDelete("{entityId}")]
    public async Task<IActionResult> RemoveAccessPermission(long entityId)
    {
        return Ok($"Access to EntityId {entityId} removed.");
    }
}
