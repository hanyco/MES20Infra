using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.Security.Exceptions;

using Microsoft.AspNetCore.Http;

namespace HanyCo.Infra.Security.Model;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public const string COMPANY_ID = "CompanyId";
    public const string PRINCIPAL_ID = "PrincipalId";
    public const string SIGNIN_TIME = "SignInTime";
    public const string USER_ID = "UserId";
    public const string USER_NAME = "UserName";

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public long CompanyId =>
        this.GetClaimValue(nameof(this.CompanyId)).Cast().ToLong();

    public bool IsLoggedIn =>
        this._httpContextAccessor.HttpContext?.User?.Claims?.Any() ?? false;

    public DateTime LastSignedInTime =>
        Convert.ToDateTime(this.GetClaimValue(nameof(this.LastSignedInTime)));

    public long PrincipalId =>
        this.GetClaimValue(nameof(this.PrincipalId)).Cast().ToLong();

    public long UserId =>
        this.GetClaimValue(nameof(this.UserId)).Cast().ToLong();

    public string UserName =>
        this.GetClaimValue(nameof(this.UserName)).ToString()!;

    public override string ToString()
        => this.IsLoggedIn ? this.UserName : "(User not logged in)";

    [return: NotNull]
    private string GetClaimValue(string claimType)
        => this.LoginGuard(() => this._httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == claimType)?.Value);

    private string GetDebuggerDisplay() =>
        this.ToString();

    [return: NotNull]
    private T LoginGuard<T>(Func<T> func) =>
        this.IsLoggedIn ? func()! : throw new NotLoggedInException();
}