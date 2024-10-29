using Application.DTOs.Identity;
using Library.Results;

namespace Application.Interfaces;

public interface IIdentityService
{
    Task<Result<TokenResponse>> GetToken(TokenRequest request, string ipAddress);

    Task<Result> Update(UpdateRequest request);
    Task<Result<string>> Remove(string Id);
    Task<Result> Register(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserInfoExResponse>> GetUser(string userId);
    Task<Result<UserInfoResponse>> GetUserCurrentUser();
    Task<Result<List<UserInfoExResponse>>> GetAllUsers();
    Task<Result<string>> ConfirmEmailAsync(string userId, string code);

    Task ForgotPassword(ForgotPasswordRequest model, string origin);

    Task<Result<string>> ResetPassword(ResetPasswordRequest model);

    Task<Result<string>> ChangePassword(ChangePasswordRequest model);

    Task<Result<string>> ChangePasswordByUser(ChangePasswordByUserRequest model);

}