namespace HanyCo.Infra.Markers;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class SecurityAttribute : Attribute
{
    public string? Key { get; set; }
    public string? Value { get; set; }
}