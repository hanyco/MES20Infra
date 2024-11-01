using Application.DTOs.Identity;
using Application.DTOs.Permissions;
using Application.Infrastructure.Persistence;
using Application.Interfaces.Shared;
using Application.Settings;

using Domain.Identity;

using Library.Data.SqlServer.Dynamics;
using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Identity;

public class IdentityService(
    IAuthenticatedUserService authenticatedUser,
    UserManager<ApplicationUser> userManager,
    IOptions<JWTSettings> jwtSettings,
    SignInManager<ApplicationUser> signInManager,
    IdentityDbContext dbContext) : IIdentityService
{
    private readonly IAuthenticatedUserService _authenticatedUser = authenticatedUser;
    private readonly JWTSettings _jwtSettings = jwtSettings.Value;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IdentityDbContext _dbContext = dbContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result> ChangePassword(ChangePasswordRequest model)
    {
        var user = await this._userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return Result.Fail($"No Accounts founded with {model.UserId}.");
        }

        var resetToken = await this._userManager.GeneratePasswordResetTokenAsync(user);
        var result = await this._userManager.ResetPasswordAsync(user, resetToken, model.Password);

        return result.Succeeded
            ? Result.Success(model.UserId, message: $"Password changed.")
            : Result.Fail($"Error occurred while changing the password.");
    }

    public async Task<Result> ChangePasswordByUser(ChangePasswordByUserRequest model)
    {
        var currentUser = this._authenticatedUser.UserId;
        var user = await this._userManager.FindByIdAsync(currentUser);
        if (user == null)
        {
            return Result.Fail($"No Accounts founded with this Id.");
        }

        _ = await this._userManager.GeneratePasswordResetTokenAsync(user);
        var result = await this._userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        await this._signInManager.RefreshSignInAsync(user);

        return result.Succeeded
            ? Result.Success(string.Empty, $"Password changed.")
            : Result.Fail($"Error occurred while changing the password.");
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string code)
    {
        var user = await this._userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Fail("User not found.", string.Empty);
        }
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await this._userManager.ConfirmEmailAsync(user, code);
        return result.Succeeded
            ? Result.Success(user.Id, message: $"Account Confirmed for {user.Email}.")
            : Result.Fail($"An error occurred while confirming {user.Email}.");
    }

    public Task ForgotPassword(ForgotPasswordRequest model, string origin) =>
        Task.CompletedTask;

    public async Task<Result<IEnumerable<UserInfoExResponse>>> GetAllUsers()
    {
        var users = await this._userManager.Users
            .ToListAsync();

        var userList = users.Select(user => new UserInfoExResponse
        {
            DisplayName = user.DisplayName,
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        });

        return Result.Success(userList);
    }

    public async Task<Result<TokenResponse?>> GetToken(TokenRequest request, string ipAddress)
    {
        try
        {
            var user = await this._userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return Result.Fail<TokenResponse?>("User not found.", default);
            }

            Check.MustBeNotNull(user, () => $"No Accounts Registered with {request.UserName}.");
            var result = await this._signInManager.PasswordSignInAsync(user.UserName!, request.Password, false, lockoutOnFailure: false);
            Check.MustBe(result.Succeeded, () => $"Invalid Credentials for '{request.UserName}'.");
            var jwtSecurityToken = await this.GenerateJWToken(user, ipAddress);
            var response = new TokenResponse
            {
                Id = user.Id,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime(),
                ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime(),
                Email = user.Email!,
                UserName = user.UserName!
            };
            var rolesList = await this._userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = [.. rolesList];
            response.IsVerified = user.EmailConfirmed;
            var refreshToken = this.GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            return Result.Success(response, "Authenticated")!;
        }
        catch (Exception ex)
        {
            return Result.Fail<TokenResponse?>(ex);
        }
    }

    public async Task<Result<UserInfoExResponse>> GetUser(string userId)
    {
        var result = await this.GetUserInfo(userId);
        return Result.Success(result);
    }

    public Task<Result<UserInfoExResponse>> GetUserCurrentUser() =>
        this.GetUser(this._authenticatedUser.UserId);

    public async Task<Result> Register(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var userWithSameEmail = await this._userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail != null)
        {
            return Result.Fail("Email is already taken.");
        }

        // Create new user object
        var user = new ApplicationUser
        {
            DisplayName = request.DisplayName,
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        // Create the user with the provided password
        var result = await this._userManager.CreateAsync(user, request.Password);

        // If user creation was not successful
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Fail($"User creation failed: {errors}");
        }

        // Optionally, add user to a role
        //if (!string.IsNullOrEmpty(request.Role))
        //{
        //    var roleResult = await this._userManager.AddToRoleAsync(user, request.Role);
        //    if (!roleResult.Succeeded)
        //    {
        //        return Result.Fail("Failed to assign role to user.");
        //    }
        //}

        // If email confirmation is required, generate and send confirmation token (optional)
        var emailConfirmationToken = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
        // You can send this token via email service if needed

        return Result.Success("User registered successfully.");
    }

    public async Task<Result> Remove(string Id)
    {
        var userWithSameId = await this._userManager.FindByIdAsync(Id);
        if (userWithSameId == null)
        {
            return Result.Fail($"UserId '{Id}' not found!");
        }
        var getUsersResult = await this.GetAllUsers();
        if (getUsersResult is { IsSucceed: false } or { Value: null })
        {
            return Result.Fail("Cannot remove user");
        }
        var users = getUsersResult.Value.ToList();
        if (users.Count <= 1)
        {
            return Result.Fail("Cannot remove user");
        }
        _ = await this._userManager.DeleteAsync(userWithSameId);
        return Result.Success("User Removed !");
    }

    public async Task<Result> ResetPassword(ResetPasswordRequest model)
    {
        var account = await this._userManager.FindByEmailAsync(model.Email);
        if (account == null)
        {
            return Result.Fail($"No Accounts Registered with {model.Email}.");
        }

        var result = await this._userManager.ResetPasswordAsync(account, model.Token, model.Password);
        return result.Succeeded
            ? Result.Success(model.Email, message: $"Password has reset.")
            : Result.Fail($"Error occurred while resetting the password.");
    }

    public async Task<Result> Update(UpdateRequest request)
    {
        try
        {
            await update(request);
            return Result.Success($"User Updated.");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex);
        }

        async Task update(UpdateRequest request)
        {
            Check.MustBeArgumentNotNull(request?.UserId);

            var user = (await this._userManager.FindByIdAsync(request.UserId)).NotNull(() => $"UserId '{request.UserId}' not found!");

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            user.DisplayName = request.DisplayName;
            user.UserName = request.UserName.NotNull(() => "User name cannot be null");
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            if (!request.Password.IsNullOrEmpty())
            {
                user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
            }

            var result = await this._userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _ = Result.Fail(result.Errors?.FirstOrDefault()?.Description ?? "Error in updating user.");
            }
        }
    }
    private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user, string ipAddress)
    {
        Check.MustBeArgumentNotNull(user);

        var userClaims = await this._userManager.GetClaimsAsync(user);
        var roles = await this._userManager.GetRolesAsync(user);
        var accessPermissions = await _dbContext.AccessPermissions
            .Where(ap => ap.UserId == user.Id)
            .ToListAsync();

        var permissionClaims = accessPermissions.Select(ap => new Claim(ap.EntityType, $"{ap.EntityId}:{ap.AccessType}"));
        var roleClaims = roles.Select(x => new Claim("roles", x));
        var result = EnumerableHelper.AsEnumerable<Claim>
        (
                new(JwtRegisteredClaimNames.Sub, user.UserName!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("uid", user.Id),
                new("full_name", $"{user.DisplayName}"),
                new("ip", ipAddress),
                new("strRoles", string.Join(",", roles))
        )
        .AddImmutedIf(!user.Email.IsNullOrEmpty(), () => new(JwtRegisteredClaimNames.Email, user.Email!))
        .AddRangeImmuted(userClaims)
        .AddRangeImmuted(roleClaims)
        .AddRangeImmuted(roles.Select(x => new Claim("roles", x)))
        .AddRangeImmuted(permissionClaims);
        return this.JWTGeneration(result);
    }

    private RefreshToken GenerateRefreshToken(string ipAddress) => new()
    {
        Token = this.RandomTokenString(),
        Expires = DateTime.UtcNow.AddDays(7),
        Created = DateTime.UtcNow,
        CreatedByIp = ipAddress
    };

    private async Task<UserInfoExResponse> GetUserInfo(string userId)
    {
        var user = await this._userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Fail<UserInfoExResponse>($"UserId '{userId}' not found!");
        }

        var result = new UserInfoExResponse()
        {
            DisplayName = user.DisplayName,
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
        return result;
    }

    private JwtSecurityToken JWTGeneration(IEnumerable<Claim> claims)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: this._jwtSettings.Issuer,
            audience: this._jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(this._jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }

    private string RandomTokenString()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(40);

        return BitConverter.ToString(randomBytes).Replace("-", "");
    }

    private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
    {
        var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var route = "api/identity/confirm-email/";
        var _endpointUri = new Uri(string.Concat($"{origin}/", route));
        var verificationUri = QueryHelpers.AddQueryString(_endpointUri.ToString(), "userId", user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
        //Email Service Call Here
        return verificationUri;
    }

    public async Task<Result> SetAccessPermissions(AccessPermissionRequest request)
    {
        // اعتبارسنجی درخواست
        Check.MustBeArgumentNotNull(request.UserId, "User ID cannot be null");
        Check.MustBeArgumentNotNull(request.EntityId, "Entity ID cannot be null");
        Check.MustBeArgumentNotNull(request.EntityType, "Entity Type cannot be null");

        // ذخیره در جدول AccessPermissions
        var permission = new AccessPermission
        {
            UserId = request.UserId,
            EntityId = request.EntityId,
            EntityType = request.EntityType,
            AccessType = request.AccessType,
            AccessScope = request.AccessScope,
            CreatedDate = DateTime.UtcNow
        };

        // اینجا باید Context یا Repository ذخیره سازی جدول AccessPermissions را صدا بزنیم
        await _dbContext.AccessPermissions.AddAsync(permission);
        await _dbContext.SaveChangesAsync();

        return Result.Success("Access permissions set successfully.");
    }

}