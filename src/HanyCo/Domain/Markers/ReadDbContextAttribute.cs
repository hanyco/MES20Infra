namespace HanyCo.Infra.Markers;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ReadDbContextAttribute : Attribute;