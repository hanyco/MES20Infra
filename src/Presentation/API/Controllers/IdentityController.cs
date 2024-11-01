using Application.DTOs.Identity;
using Application.Interfaces.Shared;

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

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await this._identityService.GetAllUsers();
        return this.Ok(result.Value);
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
        var token = await this._identityService.GetToken(tokenRequest, ipAddress);
        return token.IsSucceed
            ? this.Ok(token.Value)
            //: this.Unauthorized(new ApiErrorResponse(token.Exception?.Message, StatusCodes.Status401Unauthorized));
            : this.Unauthorized(token.Exception?.Message ?? token.Message);
    }

    [HttpPost("register")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        //var origin = this.Request.Headers.Origin.NotNull();
        var result = await this._identityService.Register(request);
        return result.IsSucceed ? this.Ok(result.Message) : this.BadRequest(result.Message);
    }

    [HttpDelete("remove/{id}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveAsync(string id)
    {
        _ = this.Request.Headers.Origin;
        var result = await this._identityService.Remove(id);
        return result.IsSucceed ? this.Ok(result.Message) : this.BadRequest(result.Message);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model) =>
        this.Ok(await this._identityService.ResetPassword(model));

    [HttpPost("update")]
    [HttpPut]
    public async Task<IActionResult> Update(UpdateRequest request)
    {
        var result = await this._identityService.Update(request);
        return result.IsSucceed ? this.Ok(result.Message) : this.BadRequest(result.Message);
    }

    [HttpGet("users/current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await this._identityService.GetUserCurrentUser();
        return result.IsSucceed ? this.Ok(result.Message) : this.BadRequest(result.Message);
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserByUserId(string userId)
    {
        var result = await this._identityService.GetUser(userId);
        return result.IsSucceed ? this.Ok(result.Message) : this.BadRequest(result.Message);
    }

    private string? GenerateIPAddress() =>
        this.Request.Headers.TryGetValue("X-Forwarded-For", out var value)
            ? (string?)value
            : this.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
}