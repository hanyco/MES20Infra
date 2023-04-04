using HanyCo.Infra.Security.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace HanyCo.Infra.Security.Model;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
internal sealed class UserContext : IUserContext
{
    #region static
    public const string USER_NAME = "UserName";
    public const string USER_ID = "UserId";
    public const string PRINCIPAL_ID = "PrincipalId";
    public const string COMPANY_ID = "CompanyId";
    public const string SIGNIN_TIME = "SignInTime";
    #endregion
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor) =>
        this._httpContextAccessor = httpContextAccessor;

    public bool IsLoggedIn => this._httpContextAccessor.HttpContext?.User?.Claims?.Any() ?? false;

    public string UserName => this.GetClaimValue(nameof(this.UserName)).ToString()!;
    public long UserId => this.GetClaimValue(nameof(this.UserId)).CastToLong();
    public long CompanyId => this.GetClaimValue(nameof(this.CompanyId)).CastToLong();
    public DateTime LastSignedInTime => Convert.ToDateTime(this.GetClaimValue(nameof(this.LastSignedInTime)));

    public long PrinicipalId => this.GetClaimValue(nameof(this.PrinicipalId)).CastToLong();

    [return: NotNull]
    private object GetClaimValue(string claimType) =>
        this.LogginGuard(() => this._httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == claimType)?.Value);

    [return: NotNull]
    private T LogginGuard<T>(Func<T> func) =>
        this.IsLoggedIn ? func()! : throw new NotLoggedInException();

    private string GetDebuggerDisplay() => this.ToString();

    public override string ToString() =>
        this.IsLoggedIn ? this.UserName : "(User not logged in)";
}
