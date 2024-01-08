namespace HanyCo.Infra.Security.Exceptions;

internal class InvalidUsernameOrPasswordException : Library.Exceptions.LibraryExceptionBase
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