namespace HanyCo.Infra.Exceptions;

public sealed class NoObjectSelectedException : MesExceptionBase
{
    public NoObjectSelectedException(string name, string? title = null, object? owner = null)
        : base($"Please select a {name}", $"No {name} selected", title, owner: owner)
    {

    }
}
