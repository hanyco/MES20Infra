using Application.DTOs.Identity;
using Library.Results;

namespace Application.Interfaces;

public interface IIdentityService
{
    Task<Result<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress);

    Task<Result<string>> UpdateAsync(UpdateRequest request);
    Task<Result<string>> RemoveAsync(string Id);
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserInfoExResponse>> UserInfoAsync(string userId);
    Task<Result<UserInfoResponse>> UserInfoAsync();
    Task<Result<List<UserInfoExResponse>>> GetAllUsersAsync();
    Task<Result<string>> ConfirmEmailAsync(string userId, string code);

    Task ForgotPassword(ForgotPasswordRequest model, string origin);

    Task<Result<string>> ResetPassword(ResetPasswordRequest model);

    Task<Result<string>> ChangePassword(ChangePasswordRequest model);

    Task<Result<string>> ChangePasswordByUser(ChangePasswordByUserRequest model);

}