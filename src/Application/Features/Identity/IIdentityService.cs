using System.Security.Claims;

using Application.DTOs.Identity;
using Application.DTOs.Permissions;

using Library.Results;

namespace Application.Features.Identity;

public interface IIdentityService
{
    Task<Result> ChangePassword(ChangePasswordRequest model);

    Task<Result> ChangePasswordByUser(ChangePasswordByUserRequest model);

    Task<Result> ConfirmEmailAsync(string userId, string code);

    Task ForgotPassword(ForgotPasswordRequest model, string origin);

    Task<Result<IEnumerable<UserInfoExResponse>>> GetAllUsers();

    Task<Result<TokenResponse?>> GetToken(TokenRequest request, string ipAddress);

    Task<Result<UserInfoExResponse>> GetUserByUserId(string userId);

    Task<Result<UserInfoExResponse>> GetUserCurrentUser();

    Task<Result> Register(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result> Remove(string Id);

    Task<Result> ResetPassword(ResetPasswordRequest model);

    Task<Result> SetAccessPermissions(AccessPermissionRequest request);

    Task<Result> Update(UpdateRequest request);

    Task<Result> AddClaimsToUser(string userId, IEnumerable<Claim> claims);
}