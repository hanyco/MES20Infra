using Application.DTOs.Identity;
using Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("[controller]")]
[ApiController]
public sealed class IdentityController:ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ISecurityService _securityService;

    public IdentityController(IIdentityService identityService, ISecurityService securityService)
    {
        this._identityService = identityService;
        this._securityService = securityService;
    }

    /// <summary>
    /// Generates a JSON Web Token for a valid combination of emailId and password.
    /// </summary>
    /// <param name="tokenRequest"></param>
    /// <returns></returns>
    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTokenAsync(TokenRequest tokenRequest)
    {
        var ipAddress = GenerateIPAddress();
        var token = await _identityService.GetTokenAsync(tokenRequest, ipAddress);
        return Ok(token);
    }

    [HttpGet("userInfo")]
    public async Task<IActionResult> UserInfo()
    {
        return Ok(await _identityService.UserInfoAsync());
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> UserInfo(string userId)
    {
        return Ok(await _identityService.UserInfoAsync(userId));
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        var origin = Request.Headers["origin"];
        return Ok(await _identityService.RegisterAsync(request, origin));
    }
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest model)
    {
        return Ok(await _identityService.ChangePassword(model));
    }
    [HttpPost("change-password-by-user")]
    public async Task<IActionResult> ChangePasswordByUserAsync(ChangePasswordByUserRequest model)
    {
        return Ok(await _identityService.ChangePasswordByUser(model));
    }
    [HttpGet("remove/{id}")]
    public async Task<IActionResult> RemoveAsync(string id)
    {
        var origin = Request.Headers["origin"];
        return Ok(await _identityService.RemoveAsync(id));
    }
    [HttpPost("update")]
    public async Task<IActionResult> update(UpdateRequest request)
    {
        return Ok(await _identityService.UpdateAsync(request));
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return Ok(await _identityService.ConfirmEmailAsync(userId, code));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        await _identityService.ForgotPassword(model, Request.Headers["origin"]);
        return Ok();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        return Ok(await _identityService.ResetPassword(model));
    }

    private string GenerateIPAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}
