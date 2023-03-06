using Library.Data.Markers;
using Library.Types;

namespace HanyCo.Infra.Security.Model;

public interface IUserBriefModel : IHasKey<Id>
{
    public string UserName { get; }
    public DateTime RegisterDate { get; }
    public DateTime? LastSignedIn { get; }
}
public interface IUserModel : IUserBriefModel, IHasKey<Id>
{
    long CompanyId { get; }
}

public interface IUserCreateModel : IUserModel
{
    string Password { get; }
}
public interface IUserUpdateModel : IUserCreateModel, IHasKey<Id>
{

}

public interface IClaimBriefModel
{

}
public interface IClaimModel : IClaimBriefModel
{

}
public interface IRoleModel : IHasKey<Id>
{

}