using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Application.DTOs.Identity;
using Application.DTOs.Permissions;
using Application.Infrastructure.Persistence;
using Application.Interfaces.Shared;
using Application.Settings;

using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.Identity;

public sealed class IdentityService(
    UserManager<AspNetUser> userManager,
    SignInManager<AspNetUser> signInManager,
    IdentityDbContext dbContext,
    IOptions<JWTSettings> jwtSettings,
    IAuthenticatedUserService authenticatedUser,
    ILogger<IdentityService> logger) : IIdentityService
{
    private readonly IAuthenticatedUserService _authenticatedUser = authenticatedUser;
    private readonly IdentityDbContext _dbContext = dbContext;
    private readonly JWTSettings _jwtSettings = jwtSettings.Value;
    private readonly ILogger<IdentityService> _logger = logger;
    private readonly SignInManager<AspNetUser> _signInManager = signInManager;
    private readonly UserManager<AspNetUser> _userManager = userManager;

    public async Task<Result> ChangePassword(ChangePasswordRequest model)
    {
        var user = await this._userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return Result.Fail($"No Accounts founded with {model.UserId}.");
        }

        this._logger.LogDebug("Changing user password by Admin. {user.UserName}", user.UserName);
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

        this._logger.LogDebug("Changing user password by user. {user.UserName}", user.UserName);
        _ = await this._userManager.GeneratePasswordResetTokenAsync(user);
        var result = await this._userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        await this._signInManager.RefreshSignInAsync(user);

        return result.Succeeded
            ? Result.Success($"Password changed.")
            : Result.Fail($"Error occurred while changing the password.");
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string code)
    {
        var user = await this._userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Fail("User not found.", string.Empty);
        }

        this._logger.LogDebug("Email confirmation. {user.UserName}", user.UserName);
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
            var user = await getUser(request);
            var response = await initResponse(ipAddress, user);
            await setRoles(user, response);
            setRefreshToken(ipAddress, response);

            return Result.Success(response, "Authenticated")!;
        }
        catch (Exception ex)
        {
            return Result.Fail<TokenResponse?>(ex);
        }

        async Task<AspNetUser> getUser(TokenRequest request)
        {
            Check.MustBeNotNull(request.UserName);
            Check.MustBeNotNull(request.Password);

            var result = await this._userManager.FindByNameAsync(request.UserName);
            Check.MustBeNotNull(result, () => $"No accounts registered with {request.UserName}.");

            var signInResult = await this._signInManager.PasswordSignInAsync(request.UserName, request.Password, false, lockoutOnFailure: false);
            Check.MustBe(signInResult.Succeeded, () => signInResult switch
            {
                { IsLockedOut: true } => "User attempting to sign-in is locked out,",
                { IsNotAllowed: true } => "User attempting to sign-in is not allowed to sign-in.",
                { RequiresTwoFactor: true } => "Uer attempting to sign-in requires two factor authentication.",
                _ => "Unknown error."
            });
            return result;
        }

        async Task<TokenResponse> initResponse(string ipAddress, AspNetUser user)
        {
            var jwtToken = await this.GenerateJWToken(user, ipAddress);
            return new TokenResponse
            {
                Id = user.Id,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                IssuedOn = jwtToken.ValidFrom.ToLocalTime(),
                ExpiresOn = jwtToken.ValidTo.ToLocalTime(),
                Email = user.Email!,
                UserName = user.UserName!,
                IsVerified = user.EmailConfirmed
            };
        }

        async Task setRoles(AspNetUser user, TokenResponse response)
        {
            var rolesList = await this._userManager.GetRolesAsync(user);
            response.Roles = [.. rolesList];
        }

        //void setRefreshToken(string ipAddress, TokenResponse response)
        //{
        //    var refreshToken = this.GenerateRefreshToken(ipAddress);
        //    response.RefreshToken = refreshToken.Token;
        //}
        void setRefreshToken(string ipAddress, TokenResponse response) =>
            response.RefreshToken = BitConverter.ToString(RandomNumberGenerator.GetBytes(40)).Replace("-", "");
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
        try
        {
            await validate(request);
            var user = await createUser(request);
            await addRolesToUser(request, user);
            var emailConfirmationToken = await this._userManager.GenerateEmailConfirmationTokenAsync(user);

            return Result.Success("User registered successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex);
        }

        async Task validate(RegisterRequest request)
        {
            var userWithSameUserName = await this._userManager.FindByNameAsync(request.UserName);
            // Check if user already exists
            if (userWithSameUserName != null)
            {
                throw new Exception("UserName is already taken.");
            }
        }

        async Task<AspNetUser> createUser(RegisterRequest request)
        {
            // Create new user object
            var user = new AspNetUser
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
                throw new Exception($"User creation failed: {errors}");
            }

            return user;
        }

        async Task addRolesToUser(RegisterRequest request, AspNetUser user) =>
            //if (!string.IsNullOrEmpty(request.Role))
            //{
            //    var roleResult = await this._userManager.AddToRoleAsync(user, request.Role);
            //    if (!roleResult.Succeeded)
            //    {
            //        throw new Exception("Failed to assign role to user.");
            //    }
            //}
            await Task.CompletedTask;
    }

    public async Task<Result> Remove(string Id)
    {
        var user = await this._userManager.FindByIdAsync(Id);
        if (user == null)
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
        _ = await this._userManager.DeleteAsync(user);
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

    public async Task<Result> SetAccessPermissions(AccessPermissionRequest request)
    {
        Check.MustBeNotNull(request.UserId, "User ID cannot be null");
        Check.MustBeNotNull(request.EntityId, "Entity ID cannot be null");
        Check.MustBeNotNull(request.EntityType, "Entity Type cannot be null");

        var permission = new AccessPermission
        {
            UserId = request.UserId,
            EntityId = request.EntityId,
            EntityType = request.EntityType,
            AccessType = request.AccessType,
            AccessScope = request.AccessScope,
            CreatedDate = DateTime.UtcNow
        };

        _ = await this._dbContext.AccessPermissions.AddAsync(permission);
        _ = await this._dbContext.SaveChangesAsync();

        return Result.Success("Access permissions set successfully.");
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

            var user = await this._userManager.FindByIdAsync(request.UserId);
            Check.MustBeNotNull(user, () => $"UserId '{request.UserId}' not found!");

            user.DisplayName = request.DisplayName;
            user.UserName = request.UserName.NotNull(() => "User name cannot be null");
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            var passwordHasher = new PasswordHasher<AspNetUser>();
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

    private async Task<JwtSecurityToken> GenerateJWToken(AspNetUser user, string ipAddress)
    {
        Check.MustBeNotNull(user);

        var userClaims = await this._userManager.GetClaimsAsync(user);
        var roles = await this._userManager.GetRolesAsync(user);
        var accessPermissionList = await this._dbContext.AccessPermissions
            .Where(ap => ap.UserId == user.Id)
            .ToListAsync();

        var permissionClaims = accessPermissionList.Select(ap => new Claim(ap.EntityType, $"{ap.EntityId}:{ap.AccessType}"));
        var roleClaims = roles.Select(x => new Claim("roles", x));
        var claims = EnumerableHelper.AsEnumerable<Claim>
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
        var result = this.JWTGeneration(claims);
        return result;
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

    private string RandomTokenString() =>
        BitConverter.ToString(RandomNumberGenerator.GetBytes(40)).Replace("-", "");

    private async Task<string> SendVerificationEmail(AspNetUser user, string origin)
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
}