using HanyCo.Infra.Security.Model;

using Library.Results;
using Library.Types;

namespace HanyCo.Infra.Security.Services;

public interface IInfraUserService
{
    Result<IEnumerable<IUserBriefModel>> GetAll();
    Task<Result<IUserModel?>> GetByIdAsync(Id id);
    Task<Result<IUserModel?>> GetByUserNameAsync(string userName);

    Task<Result> CreateAsync(IUserCreateModel user);
    Task<Result> UpdateAsync(IUserUpdateModel user);
    Task<Result> DeleteAsync(Id userId);

    /// <summary>
    /// Confirms the new user's email.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="token">The token.</param>
    /// <exception cref="ArgumentException">
    /// '{nameof(userName)}' cannot be null or empty - userName
    /// or
    /// '{nameof(token)}' cannot be null or empty - token
    /// </exception>
    /// <exception cref="ObjectNotFoundException">User not found.</exception>
    /// <exception cref="BadRequestException">Email not confirmed.</exception>
    Task<Result> ConfirmEmailAsync(string userName, string token);
    Task<Result<string>> SignInByPasswordAsync(string userName, string password, bool isPersistent = false);
    Task<Result> SignOutAsync();

    Task<Result> SendPasswordResetEmailTokenAsync(string userName);
    /// <summary>
    /// Sends the password reset email token asynchronous.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <exception cref="ArgumentException">'{nameof(userName)}' cannot be null or empty - userName</exception>
    /// <exception cref="ObjectNotFoundException">User not found.</exception>Task<Result> SendPasswordResetEmailTokenAsync(string userName);
    Task<Result> ConfirmPasswordResetTokenAsync(string userName, string token, string newPassword);
}

