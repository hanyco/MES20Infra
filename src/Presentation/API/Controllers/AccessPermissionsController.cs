using Application.DTOs.Permissions;
using Application.Interfaces.Shared;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
public class AccessPermissionsController(IIdentityService identityService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SetAccessPermissions([FromBody] AccessPermissionRequest request)
    {
        var result = await identityService.SetAccessPermissions(request);
        if (!result.IsSucceed)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }
}
