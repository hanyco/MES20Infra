namespace HanyCo.Infra.Security.Model;

public interface IUserContext
{
    bool IsLoggedIn { get; }
    string UserName { get; }
    long UserId { get; }
    long CompanyId { get; }
    DateTime LastSignedInTime { get; }
    long PrinicipalId { get; }
}