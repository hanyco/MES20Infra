using Domain.Identity;
using Library.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using Application.Interfaces.Shared;
using Application.DTOs.Settings;
using Application.DTOs.Identity;
using Library.Validations;
using HanyCo.Infra.Exceptions;
using Application.Interfaces;

namespace Application.Identity.Services;
public class IdentityService(
    IAuthenticatedUserService authenticatedUser,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<JWTSettings> jwtSettings,
    SignInManager<ApplicationUser> signInManager,
    IDistributedCache distributedCache) : IIdentityService
{
    private readonly IAuthenticatedUserService _authenticatedUser = authenticatedUser;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly JWTSettings _jwtSettings = jwtSettings.Value;
    private readonly IDistributedCache _distributedCache = distributedCache;

    private async Task ClearCache()
    {
        await _distributedCache.RemoveAsync("Users");
    }

    public async Task<Result<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        Check.MustBeNotNull(user, () => $"No Accounts Registered with {request.UserName}.");
        var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
        //Throw.Exception.IfFalse(user.EmailConfirmed, $"Email is not confirmed for '{request.UserName}'.");
        //Throw.Exception.IfFalse(user.IsActive, $"Account for '{request.UserName}' is not active. Please contact the Administrator.");
        Check.MustBe(result.Succeeded, () => $"Invalid Credentials for '{request.UserName}'.");
        JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
        var response = new TokenResponse
        {
            Id = user.Id,
            JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime(),
            ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime(),
            Email = user.Email,
            UserName = user.UserName
        };
        var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        response.Roles = rolesList.ToList();
        response.IsVerified = user.EmailConfirmed;
        var refreshToken = GenerateRefreshToken(ipAddress);
        response.RefreshToken = refreshToken.Token;
        return Result.Success(response, "Authenticated");
    }

    private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user, string ipAddress)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        for (int i = 0; i < roles.Count; i++)
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
                new Claim("strRoles", String.Join(",",roles)),
            }
        .Union(userClaims)
        .Union(roleClaims);
        return JWTGeneration(claims);
    }

    private JwtSecurityToken JWTGeneration(IEnumerable<Claim> claims)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
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

    private RefreshToken GenerateRefreshToken(string ipAddress)
    {
        return new RefreshToken
        {
            Token = RandomTokenString(),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }
    public async Task<Result<string>> UpdateAsync(UpdateRequest request)
    {
        //if (!_authenticatedUser.User.IsInRole(Roles.SuperAdmin.ToString()))
        //{
        //    throw new MesException($"Only SuperAdmin Can Update User !");
        //}
        // request.Role = Roles.Moderator;
        var userWithSameId = await _userManager.FindByIdAsync(request.Id) ?? throw new MesException($"UserId '{request.Id}' not found!");
        userWithSameId.DisplayName = request.DisplayName;
        userWithSameId.UserName = request.UserName;
        userWithSameId.Email = request.Email;
        userWithSameId.PhoneNumber = request.PhoneNumber;
        
        var result = await _userManager.UpdateAsync(userWithSameId);
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

    public async Task<Result<string>> RemoveAsync(string Id)
    {
        //if (!_authenticatedUser.User.IsInRole(Roles.SuperAdmin.ToString()))
        //{
        //    throw new MesException($"Only SuperAdmin Can Remove User !");
        //}
        var userWithSameId = await _userManager.FindByIdAsync(Id);
        if (userWithSameId == null)
        {
            throw new MesException($"UserId '{Id}' not found!");
        }
        await _userManager.DeleteAsync(userWithSameId);
        await ClearCache();
        return Result.Success<string>("User Removed !");
    }

    public async Task<Result<UserInfoResponse>> UserInfoAsync()
    {
        return Result.Success<UserInfoResponse>(await GetUserInfo(_authenticatedUser.UserId));
    }
    public async Task<Result<UserInfoExResponse>> UserInfoAsync(string userId)
    {
        var result = await GetUserInfo(userId);
        return Result.Success<UserInfoExResponse>(result);
    }

    private async Task<UserInfoExResponse> GetUserInfo(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
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

    public async Task<Result<string>> RegisterAsync(RegisterRequest request, string origin)
    {
        //request.Role = Roles.Basic;
        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            throw new MesException($"Username '{request.UserName}' is already taken.");
        }
        if (request.ProfilePicture == null)
        {
            request.ProfilePicture = "%5CUsers%5Cuser-placeholder.jpg";
        }
        var user = new ApplicationUser
        {
            Email = request.Email,
            DisplayName = request.DisplayName,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {

            //await _userManager.AddToRoleAsync(user, request.Role.ToString());
            await ClearCache();

            //return Result.Success<string>(user.Id, message: $"User {request.Role} Registered.");
            return Result.Success<string>(user.Id, message: $"User Registered.");

        }
        else
        {

            throw new MesException($"{result.Errors}");
        }
    }

    private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var route = "api/identity/confirm-email/";
        var _enpointUri = new Uri(string.Concat($"{origin}/", route));
        var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
        //Email Service Call Here
        return verificationUri;
    }

    public async Task<Result<string>> ConfirmEmailAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            return Result.Success<string>(user.Id, message: $"Account Confirmed for {user.Email}. You can now use the /api/identity/token endpoint to generate JWT.");
        }
        else
        {
            throw new MesException($"An error occurred while confirming {user.Email}.");
        }
    }

    public Task ForgotPassword(ForgotPasswordRequest model, string origin)
    {
        return Task.CompletedTask;
    }

    public async Task<Result<string>> ResetPassword(ResetPasswordRequest model)
    {
        var account = await _userManager.FindByEmailAsync(model.Email);
        if (account == null) throw new MesException($"No Accounts Registered with {model.Email}.");
        var result = await _userManager.ResetPasswordAsync(account, model.Token, model.Password);
        if (result.Succeeded)
        {
            return Result.Success<string>(model.Email, message: $"Password Resetted.");
        }
        else
        {
            throw new MesException($"Error occurred while resetting the password.");
        }
    }
    public async Task<Result<string>> ChangePassword(ChangePasswordRequest model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) throw new MesException($"No Accounts founded with {model.Id}.");
        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, model.Password);

        if (result.Succeeded)
        {
            return Result.Success<string>(model.Id, message: $"Password changed.");
        }
        else
        {
            throw new MesException($"Error occured while changing the password.");
        }
    }

    public async Task<Result<string>> ChangePasswordByUser(ChangePasswordByUserRequest model)
    {
        var currentUser = _authenticatedUser.UserId;
        var user = await _userManager.FindByIdAsync(currentUser);
        if (user == null) throw new MesException($"No Accounts founded with this Id.");
        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        await _signInManager.RefreshSignInAsync(user);

        if (result.Succeeded)
        {
            return Result.Success<string>(string.Empty, $"Password changed.");
        }
        else
        {
            throw new MesException($"Error occured while changing the password.");
        }
    }
}