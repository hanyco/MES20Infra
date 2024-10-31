using Application.DTOs.Identity;

using Library.Results;

namespace Application.Interfaces;

public interface IIdentityService
{
    Task<Result<string>> ChangePassword(ChangePasswordRequest model);

    Task<Result<string>> ChangePasswordByUser(ChangePasswordByUserRequest model);

    Task<Result<string>> ConfirmEmailAsync(string userId, string code);

    Task ForgotPassword(ForgotPasswordRequest model, string origin);

    Task<Result<List<UserInfoExResponse>>> GetAllUsers();

    Task<Result<TokenResponse>> GetToken(TokenRequest request, string ipAddress);

    Task<Result<UserInfoExResponse>> GetUser(string userId);

    Task<Result<UserInfoExResponse>> GetUserCurrentUser();

    Task<Result> Register(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<string>> Remove(string Id);

    Task<Result<string>> ResetPassword(ResetPasswordRequest model);

    Task<Result> Update(UpdateRequest request);
}