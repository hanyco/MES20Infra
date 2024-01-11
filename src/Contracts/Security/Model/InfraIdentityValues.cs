namespace HanyCo.Infra.Security.Model;

public static class InfraIdentityValues
{
    public const string ClaimFirstName = "FirstName";
    public const string ClaimLastName = "LastName";
    public const string LoggedInAuthenticationType = "MES Infra Authentication Type";
    public const string PolicyCanCrudSystemEntities = "CanSystemCreate";
    public const string PolicyCanSystemDelete = "CanSystemDelete";
    public const string PolicyCanViewSystemEntities = "CanViewSystemEntities";
    public const string PolicyIsAdmin = "IsAdmin";
    public const string RoleAdminValue = "Administrators";
    public const string RoleAnonymous = "Anonymous";
    public const string RoleSupervisor = "Supervisor";
    public const string RoleUser = "User";
}