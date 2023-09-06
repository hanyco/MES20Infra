using System.Diagnostics;
using System.Security.Claims;

using HanyCo.Infra.Security.Model;
using HanyCo.Infra.Security.Services;

using Library.Coding;
using Library.Exceptions;
using Library.Mapping;
using Library.Results;
using Library.Security.Claims;
using Library.Types;
using Library.Validations;

namespace HanyCo.Infra.Security.Identity;

internal sealed class InfraIdentityService : ISecurityService, IInfraUserService
{
    private readonly ClaimsIdentity _claimManager;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<InfraIdentityUser> _passwordHasher;
    private readonly RoleManager<InfraIdentityRole> _roleManager;
    private readonly SignInManager<InfraIdentityUser> _signInManager;
    private readonly UserManager<InfraIdentityUser> _userManager;

    public InfraIdentityService(UserManager<InfraIdentityUser> userManager,
                                SignInManager<InfraIdentityUser> signInManager,
                                RoleManager<InfraIdentityRole> roleManager,
                                ClaimsIdentity claimManager,
                                IMapper mapper,
                                IPasswordHasher<InfraIdentityUser> passwordHasher)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._roleManager = roleManager;
        this._claimManager = claimManager;
        this._mapper = mapper;
        this._passwordHasher = passwordHasher;
    }

    public async Task<Result> AddClaimToUserAsync(string userId, string claimType, string? claimValue)
    {
        Check.MustBeArgumentNotNull(claimType);
        var user = await this.GetUserByIdAsync(userId);
        if (!user.IsSucceed)
        {
            return Result.Failure;
        }
        var claim = this.GetOrCreate(claimType, claimValue, true);

        var result = await this._userManager.AddClaimAsync(user!, claim);
        return result.ToResult();
    }

    public async Task<Result> AddUserToRoleAsync(string userId, string roleId)
    {
        var user = await this.GetUserByIdAsync(userId);
        if (!user.IsSucceed)
        {
            return Result.CreateFailure(message: "User not found");
        }
        var role = await this.GetRoleByIdAsync(roleId);
        if (!role.IsSucceed)
        {
            return Result.CreateFailure(message: "Role not found");
        }
        var result = await this._userManager.AddToRoleAsync(user!, role.Value!.Name);
        return result.ToResult();
    }

    public async Task<Result> ConfirmPasswordResetTokenAsync(string userName, string token, string newPassword)
    {
        var user = await this.FindByNameAsync(userName);
        if (!user.IsSucceed)
        {
            return Result.Failure;
        }
        var result = await this._userManager.ResetPasswordAsync(user!, token, newPassword);
        return result.ToResult();
    }

    public async Task<Result> ConfirmEmailAsync(string userName, string token)
    {
        var findUserResult = await this.FindByNameAsync(userName);
        if (!findUserResult.IsSucceed)
        {
            return Result.CreateFailure(message: findUserResult?.Message ?? "User not found.");
        }

        var confirmEmailResult = await this._userManager.ConfirmEmailAsync(findUserResult!, token);
        if (confirmEmailResult.Succeeded)
        {
            //AddClaimToUserAsync()
        }
        return confirmEmailResult.ToResult();
    }

    public async Task<Result> CreateAsync(IUserCreateModel user)
    {
        var infraUser = this._mapper.Map<InfraIdentityUser>(user)
                                    .ForMember(x => x.PasswordHash = this._passwordHasher.HashPassword(x, user.Password))
                                    .ForMember(x => x.RegisterDate = DateTime.Now);
        var result = await this._userManager.CreateAsync(infraUser, user.Password);
        if (result.Succeeded)
        {
            var confirmationToken = await this._userManager.GenerateEmailConfirmationTokenAsync(infraUser);
            _ = await this.SendConfirmationMailAsync(infraUser.UserName, confirmationToken);
        }
        return result.ToResult();
    }

    public async Task<Result> DeleteAsync(Id userId)
    {
        var user = await this.GetUserByIdAsync(userId.ToString());
        if (user is null or { Value: null })
        {
            return Result.Failure;
        }
        var result = await this._userManager.DeleteAsync(user!);
        return result.ToResult();
    }

    public Result<IEnumerable<IUserBriefModel>> GetAll()
                                => Result<IEnumerable<IUserBriefModel>>.CreateSuccess(this._userManager.Users.AsEnumerable());

    public async Task<Result<IUserModel?>> GetByIdAsync(Id id)
        => Result<IUserModel?>.CreateSuccess(await this._userManager.FindByIdAsync(id.ToString()));

    public async Task<Result<IUserModel?>> GetByUserNameAsync(string userName)
        => Result<IUserModel?>.CreateSuccess(await this._userManager.FindByNameAsync(userName));

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        Check.MustBeArgumentNotNull(userId, nameof(userId));

        var user = await this._userManager.FindByIdAsync(userId) ?? throw new ObjectNotFoundException($"User {userId} not found.");
        var result = await this._userManager.GetRolesAsync(user);
        return EnumerableHelper.DefaultIfNull(result);
    }

    public async Task<bool> HasUserClaim(string userId, string claimType)
    {
        Check.MustBeArgumentNotNull(userId);
        Check.MustBeArgumentNotNull(claimType);
        var user = await this.GetUserByIdAsync(userId);
        var userClaims = await this._userManager.GetClaimsAsync(user!);
        return userClaims.Any(c => c.Type == claimType);
    }

    public async Task<bool> IsInRoleAsync(string userId, string roleId)
    {
        var user = await this.GetUserByIdAsync(userId);
        var role = await this.GetRoleByIdAsync(roleId);
        var result = await this._userManager.IsInRoleAsync(user!, role.Value!.Name);
        return result;
    }

    public bool IsSignedIn(ClaimsPrincipal user) =>
        this._signInManager.IsSignedIn(user);

    public bool IsSignedIn(string token) => throw new NotImplementedException();

    public async Task<Result> RemoveClaimFromUserAsync(string claimType, string userId)
    {
        Check.MustBeArgumentNotNull(claimType);
        var user = await this.GetUserByIdAsync(userId);
        if (!user.IsSucceed)
        {
            return Result.Failure;
        }
        var claim = this.GetOrCreate(claimType, Permission.FullAccess.ToString());
        var result = await this._userManager.RemoveClaimAsync(user!, claim);
        return result.ToResult();
    }

    public async Task<Result> RemoveUserFromRoleAsync(string userId, string roleId)
    {
        var user = await this.GetUserByIdAsync(userId);
        if (!user.IsSucceed)
        {
            return Result.CreateFailure(message: "User not found");
        }
        var role = await this.GetRoleByIdAsync(roleId);

        if (!role.IsSucceed)
        {
            return Result.CreateFailure(message: "Role not found");
        }
        if (!await this._userManager.IsInRoleAsync(user!, role.Value!.Name))
        {
            return Result.Failure;
        }

        var result = await this._userManager.RemoveFromRoleAsync(user!, role.Value!.Name);
        return result.ToResult();
    }

    public async Task<Result> SendPasswordResetEmailTokenAsync(string userName)
    {
        var user = await this.FindByNameAsync(userName);
        if (!user.IsSucceed)
        {
            return Result.CreateFailure(message: user?.Message ?? "User not found.");
        }

        _ = await this._userManager.GeneratePasswordResetTokenAsync(user!);
        return await this.SendPasswordResetEmailTokenAsync(user.Value!.UserName);
    }

    public async Task<Result<string>> SignInByPasswordAsync(string userId, string password, bool isPersistent = false)
    {
        Check.MustBeArgumentNotNull(password);

        var result = await this._signInManager.PasswordSignInAsync(userId, password, isPersistent, true);
        if (!result.Succeeded)
        {
            check(result.IsLockedOut, "This user is locked. Please try again later.");
            check(result.IsNotAllowed, "This user attempting to sign-in is not allowed.");
            check(result.RequiresTwoFactor, "This user attempting to sign-in requires two factor authentication.");
            check(false, "Invalid username or password.");
        }
        var token = Guid.NewGuid().ToString(); // Undone
        return new(token);

        [StackTraceHidden]
        void check(bool required, string message) => Check.MustBe(required, () => new UnauthorizedException(message, owner: this));
    }

    public async Task<Result> SignOutAsync()
    {
        await this._signInManager.SignOutAsync();
        return Result.CreateSuccess();
    }

    public async Task<Result> UpdateAsync(IUserUpdateModel user)
    {
        Check.MustBeArgumentNotNull(user);
        var dbUser = await this.GetUserByIdAsync(user.Id.ToString());
        if (!dbUser.IsSucceed)
        {
            return Result.Failure;
        }
        var idnUser = dbUser.Value!;
        idnUser.UserName = user.UserName;
        var result = await this._userManager.UpdateAsync(idnUser);
        return result.ToResult();
    }

    public bool UserHasClaim(ClaimsPrincipal user, ClaimInfo claim) =>
        user.ArgumentNotNull().HasClaim(claim.Type, claim.Value ?? LibClaimDefaultValues.VALID_CLAIM_VALUE);

    public bool UserHasClaim(string token, ClaimInfo claim) => throw new NotImplementedException();

    private async Task<Result<InfraIdentityUser?>> FindByNameAsync(string userName)
    {
        Check.MustBeArgumentNotNull(userName);

        var user = await this._userManager.FindByNameAsync(userName);
        return user is null ? Result<InfraIdentityUser>.Failure : Result<InfraIdentityUser?>.CreateSuccess(user);
    }

    private Claim GetOrCreate(string claimType, string? claimValue, bool createIfNotFound = true)
    {
        var claim = this._claimManager.Claims.SingleOrDefault(x => x.Type == claimType);
        if (claim is null)
        {
            Check.MustBe(!createIfNotFound, () => new ObjectNotFoundException("Claim type not found."));

            claim = new Claim(claimType, claimValue ?? LibClaimDefaultValues.VALID_CLAIM_VALUE);
            this._claimManager.AddClaim(claim);
        }

        return claim;
    }

    private async Task<Result<InfraIdentityRole?>> GetRoleByIdAsync(string roleId)
    {
        Check.MustBeArgumentNotNull(roleId);
        var role = await this._roleManager.FindByIdAsync(roleId);
        return role is null ? Result<InfraIdentityRole>.Failure : Result<InfraIdentityRole?>.CreateSuccess(role);
    }

    private async Task<Result<InfraIdentityUser?>> GetUserByIdAsync(string userId)
        => await this.Fluent(() => this._userManager.FindByIdAsync(userId),
            user => user is null ? Result<InfraIdentityUser>.Failure : Result<InfraIdentityUser?>.CreateSuccess(user));

    private async Task<Result> SendConfirmationMailAsync(string userName, string confirmationToken) // Undone
        => await Task.FromResult(Result.CreateSuccess(message: "Confirmation mail sent, if the email address was valid."));
}