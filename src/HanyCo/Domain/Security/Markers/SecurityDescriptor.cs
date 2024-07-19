namespace HanyCo.Infra.Security.Markers;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class SecurityDescriptorAttribute : Attribute
{
    public string? Key { get; set; }
}