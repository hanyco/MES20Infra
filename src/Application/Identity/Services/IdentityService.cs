﻿using Application.DTOs.Identity;
using Application.DTOs.Settings;
using Application.Interfaces;
using Application.Interfaces.Shared;

using Domain.Identity;

using HanyCo.Infra.Exceptions;

using Library.Results;
using Library.Validations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Identity.Services;

public class IdentityService(
    IAuthenticatedUserService authenticatedUser,
    UserManager<ApplicationUser> userManager,
    IOptions<JWTSettings> jwtSettings,
    SignInManager<ApplicationUser> signInManager,
    IDistributedCache distributedCache) : IIdentityService
{
    private readonly IAuthenticatedUserService _authenticatedUser = authenticatedUser;
    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly JWTSettings _jwtSettings = jwtSettings.Value;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<string>> ChangePassword(ChangePasswordRequest model)
    {
        var user = await this._userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            throw new MesException($"No Accounts founded with {model.UserId}.");
        }

        var resetToken = await this._userManager.GeneratePasswordResetTokenAsync(user);
        var result = await this._userManager.ResetPasswordAsync(user, resetToken, model.Password);

        return result.Succeeded
            ? Result.Success<string>(model.UserId, message: $"Password changed.")
            : throw new MesException($"Error occured while changing the password.");
    }

    public async Task<Result<string>> ChangePasswordByUser(ChangePasswordByUserRequest model)
    {
        var currentUser = this._authenticatedUser.UserId;
        var user = await this._userManager.FindByIdAsync(currentUser);
        if (user == null)
        {
            throw new MesException($"No Accounts founded with this Id.");
        }

        _ = await this._userManager.GeneratePasswordResetTokenAsync(user);
        var result = await this._userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        await this._signInManager.RefreshSignInAsync(user);

        return result.Succeeded
            ? Result.Success<string>(string.Empty, $"Password changed.")
            : throw new MesException($"Error occured while changing the password.");
    }

    public async Task<Result<string>> ConfirmEmailAsync(string userId, string code)
    {
        var user = await this._userManager.FindByIdAsync(userId);
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await this._userManager.ConfirmEmailAsync(user, code);
        return result.Succeeded
            ? Result.Success<string>(user.Id, message: $"Account Confirmed for {user.Email}. You can now use the /api/identity/token endpoint to generate JWT.")
            : throw new MesException($"An error occurred while confirming {user.Email}.");
    }

    public Task ForgotPassword(ForgotPasswordRequest model, string origin) => Task.CompletedTask;

    public async Task<Result<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress)
    {
        try
        {
            var user = await this._userManager.FindByNameAsync(request.UserName);
            Check.MustBeNotNull(user, () => $"No Accounts Registered with {request.UserName}.");
            var result = await this._signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            Check.MustBe(result.Succeeded, () => $"Invalid Credentials for '{request.UserName}'.");
            var jwtSecurityToken = await this.GenerateJWToken(user, ipAddress);
            var response = new TokenResponse
            {
                Id = user.Id,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime(),
                ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime(),
                Email = user.Email,
                UserName = user.UserName
            };
            var rolesList = await this._userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            var refreshToken = this.GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            return Result.Success(response, "Authenticated");
        }
        catch (Exception ex)
        {
            return Result.Fail<TokenResponse>(ex);
        }
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
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
        if (!string.IsNullOrEmpty(request.Role))
        {
            var roleResult = await this._userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                return Result.Fail("Failed to assign role to user.");
            }
        }

        // If email confirmation is required, generate and send confirmation token (optional)
        var emailConfirmationToken = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
        // You can send this token via email service if needed

        return Result.Success("User registered successfully.");
    }

    public async Task<Result<string>> RemoveAsync(string Id)
    {
        //if (!_authenticatedUser.User.IsInRole(Roles.SuperAdmin.ToString()))
        //{
        //    throw new MesException($"Only SuperAdmin Can Remove User !");
        //}
        var userWithSameId = await this._userManager.FindByIdAsync(Id);
        if (userWithSameId == null)
        {
            throw new MesException($"UserId '{Id}' not found!");
        }
        _ = await this._userManager.DeleteAsync(userWithSameId);
        await this.ClearCache();
        return Result.Success<string>("User Removed !");
    }

    public async Task<Result<string>> ResetPassword(ResetPasswordRequest model)
    {
        var account = await this._userManager.FindByEmailAsync(model.Email);
        if (account == null)
        {
            throw new MesException($"No Accounts Registered with {model.Email}.");
        }

        var result = await this._userManager.ResetPasswordAsync(account, model.Token, model.Password);
        return result.Succeeded
            ? Result.Success<string>(model.Email, message: $"Password Resetted.")
            : throw new MesException($"Error occurred while resetting the password.");
    }

    public async Task<Result<string>> UpdateAsync(UpdateRequest request)
    {
        //if (!_authenticatedUser.User.IsInRole(Roles.SuperAdmin.ToString()))
        //{
        //    throw new MesException($"Only SuperAdmin Can Update User !");
        //}
        // request.Role = Roles.Moderator;
        var userWithSameId = await this._userManager.FindByIdAsync(request.Id) ?? throw new MesException($"UserId '{request.Id}' not found!");
        userWithSameId.DisplayName = request.DisplayName;
        userWithSameId.UserName = request.UserName;
        userWithSameId.Email = request.Email;
        userWithSameId.PhoneNumber = request.PhoneNumber;

        var result = await this._userManager.UpdateAsync(userWithSameId);
        if (!result.Succeeded)
        {
            throw new MesException($"User Update Error !");
        }
        //   var roles = await _userManager.GetRolesAsync(userWithSameId);
        // await _userManager.RemoveFromRolesAsync(userWithSameId, roles);
        //await _userManager.AddToRoleAsync(userWithSameId, request.Role.ToString());
        //await ClearCache();
        return Result.Success<string>($"User Updated.");
    }

    public async Task<Result<UserInfoResponse>> UserInfoAsync() =>
        Result.Success<UserInfoResponse>(await this.GetUserInfo(this._authenticatedUser.UserId));

    public async Task<Result<UserInfoExResponse>> UserInfoAsync(string userId)
    {
        var result = await this.GetUserInfo(userId);
        return Result.Success<UserInfoExResponse>(result);
    }

    private async Task ClearCache() => await this._distributedCache.RemoveAsync("Users");

    private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user, string ipAddress)
    {
        var userClaims = await this._userManager.GetClaimsAsync(user);
        var roles = await this._userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        for (var i = 0; i < roles.Count; i++)
        {
            roleClaims.Add(new Claim("roles", roles[i]));
        }
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                //new Claim("first_name", user.FirstName),
                //new Claim("last_name", user.LastName),
                new Claim("full_name", $"{user.DisplayName}"),
                new Claim("ip", ipAddress),
                new Claim("strRoles", string.Join(",",roles)),
            }
        .Union(userClaims)
        .Union(roleClaims);
        return this.JWTGeneration(claims);
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
            throw new MesException($"UserId '{userId}' not found!");
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
        using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        var randomBytes = new byte[40];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        // convert random bytes to hex string
        return BitConverter.ToString(randomBytes).Replace("-", "");
    }

    public async Task<Result<List<UserInfoExResponse>>> GetAllUsersAsync()
    {
        var users = await this._userManager.Users.ToListAsync();

        var userList = users.Select(user => new UserInfoExResponse
        {
            DisplayName = user.DisplayName,
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        }).ToList();

        return Result.Success(userList);
    }


    private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
    {
        var code = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var route = "api/identity/confirm-email/";
        var _enpointUri = new Uri(string.Concat($"{origin}/", route));
        var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
        //Email Service Call Here
        return verificationUri;
    }
}