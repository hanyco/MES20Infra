namespace HanyCo.Infra.Security.Exceptions;

[Serializable]
public sealed class NotLoggedInException : Library.Exceptions.LibraryExceptionBase
{
    public NotLoggedInException()
    {
    }

    public NotLoggedInException(string message) : base(message)
    {
    }

    public NotLoggedInException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public NotLoggedInException(string message, string? instruction = null, string? title = null, string? details = null, Exception? inner = null, object? owner = null) : base(message, instruction, title, details, inner, owner)
    {
    }

    private NotLoggedInException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) => throw new NotImplementedException();
}