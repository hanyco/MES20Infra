using System.Security.Claims;

using Application.DTOs.Identity;
using Application.DTOs.Permissions;

using Library.Results;

namespace Application.Features.Identity;

public interface IIdentityService
{
    /// <summary>
    /// Change the password of the current user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<Result> ChangePassword(ChangePasswordRequest model);

    /// <summary>
    /// Change the password of a user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<Result> ChangePasswordByUser(ChangePasswordByUserRequest model);

    /// <summary>
    /// Confirm the email of a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<Result> ConfirmEmailAsync(string userId, string code);

    /// <summary>
    /// Send a password reset email to the user.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    Task ForgotPassword(ForgotPasswordRequest model, string origin);

    /// <summary>
    /// Retrieve all users.
    /// </summary>
    /// <returns></returns>
    Task<Result<IEnumerable<UserInfoExResponse>>> GetAllUsers();

    /// <summary>
    /// Retrieve a JSON Web Token for a valid combination of emailId and password.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    Task<Result<TokenResponse?>> GetToken(TokenRequest request, string ipAddress);

    /// <summary>
    /// Retrieve a user by their ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Result<UserInfoExResponse>> GetUserByUserId(string userId);

    /// <summary>
    /// Retrieve the current user.
    /// </summary>
    /// <returns></returns>
    Task<Result<UserInfoExResponse>> GetUserCurrentUser();

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> Register(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a user.
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    Task<Result> Remove(string Id);

    /// <summary>
    /// Reset the password of a user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<Result> ResetPassword(ResetPasswordRequest model);

    /// <summary>
    /// Set the access permissions for a user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Result> SetAccessPermissions(AccessPermissionRequest request);

    /// <summary>
    /// Update the details of a user.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Result> Update(UpdateRequest request);

    /// <summary>
    /// Add claims to a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="claims"></param>
    /// <returns></returns>
    Task<Result> AddClaimsToUser(string userId, IEnumerable<Claim> claims);
}