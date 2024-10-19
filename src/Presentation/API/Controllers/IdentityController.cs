using Application.DTOs.Identity;
using Application.Interfaces;

using Domain.Dtos;

using Library.Validations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class IdentityController(IIdentityService identityService, ISecurityService securityService) : ControllerBase
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ISecurityService _securityService = securityService;

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest model) =>
        this.Ok(await this._identityService.ChangePassword(model));

    [HttpPost("change-password-by-user")]
    public async Task<IActionResult> ChangePasswordByUserAsync(ChangePasswordByUserRequest model) =>
        this.Ok(await this._identityService.ChangePasswordByUser(model));

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code) =>
        this.Ok(await this._identityService.ConfirmEmailAsync(userId, code));

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        await this._identityService.ForgotPassword(model, this.Request.Headers.Origin.NotNull());
        return this.Ok();
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
        var ipAddress = this.GenerateIPAddress().NotNull(() => "Cannot find local address");
        var token = await this._identityService.GetTokenAsync(tokenRequest, ipAddress);
        if (token.IsSucceed)
            return this.Ok(token.Value);
        else
            return this.Unauthorized(new ApiErrorResponse(token.Exception.Message, StatusCodes.Status401Unauthorized));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        //var origin = this.Request.Headers.Origin.NotNull();
        var result = await _identityService.RegisterAsync(request);
        if (result.IsSucceed)
            return Ok(result.Message);
        else
            return BadRequest(result.Message);
    }

    [HttpGet("remove/{id}")]
    public async Task<IActionResult> RemoveAsync(string id)
    {
        _ = this.Request.Headers.Origin;
        return this.Ok(await this._identityService.RemoveAsync(id));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model) =>
        this.Ok(await this._identityService.ResetPassword(model));

    [HttpPost("update")]
    public async Task<IActionResult> Update(UpdateRequest request) =>
        this.Ok(await this._identityService.UpdateAsync(request));

    [HttpGet("userInfo")]
    public async Task<IActionResult> UserInfo() =>
        this.Ok(await this._identityService.UserInfoAsync());

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> UserInfo(string userId) =>
        this.Ok(await this._identityService.UserInfoAsync(userId));

    private string? GenerateIPAddress() =>
        this.Request.Headers.TryGetValue("X-Forwarded-For", out var value)
            ? (string?)value
            : this.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
}