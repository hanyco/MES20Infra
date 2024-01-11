namespace HanyCo.Infra.Security.Exceptions;

[Serializable]
public sealed class InvalidUsernameOrPasswordException : SecurityException
{
    public InvalidUsernameOrPasswordException(string message) : base(message)
    {
    }

    public InvalidUsernameOrPasswordException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public InvalidUsernameOrPasswordException() : base("Invalid username or password")
    {
    }
}

[Serializable]
public abstract class SecurityException : Library.Exceptions.LibraryExceptionBase
{
    protected SecurityException(string message) : base(message)
    {
    }

    protected SecurityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SecurityException()
    {
    }
}

[Serializable]
public sealed class UserCannotLoginException : SecurityException
{
    public UserCannotLoginException(string message) : base(message)
    {
    }

    public UserCannotLoginException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public UserCannotLoginException()
    {
    }
}

[Serializable]
public sealed class UserIsLockedOutException : SecurityException
{
    public UserIsLockedOutException(string message) : base(message)
    {
    }

    public UserIsLockedOutException()
    {
    }

    public UserIsLockedOutException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

[Serializable]
public sealed class NoUserFoundException : SecurityException
{
    public NoUserFoundException(string message) : base(message)
    {
    }

    public NoUserFoundException()
    {
    }

    public NoUserFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}