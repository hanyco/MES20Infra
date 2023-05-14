namespace HanyCo.Infra.Markers;

[AttributeUsage(AttributeTargets.All)]
public sealed class SecurityDescriptorAttribute : Attribute
{
    public SecurityDescriptorAttribute(Guid guid)
        => this.Guid = guid;

    public Guid Guid { get; }
}