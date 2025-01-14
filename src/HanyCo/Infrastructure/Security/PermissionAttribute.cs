namespace HanyCo.Infra.Security;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class PermissionAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}