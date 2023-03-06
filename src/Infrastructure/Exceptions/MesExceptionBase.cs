using Library.Exceptions;

namespace HanyCo.Infra.Exceptions
{
    public abstract class MesExceptionBase : ExceptionBase, IMesException
    {
        protected MesExceptionBase()
        {
        }

        protected MesExceptionBase(string message) : base(message)
        {
        }

        protected MesExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MesExceptionBase(string message, string? instruction = null, string? title = null, string? details = null, Exception? inner = null, object? owner = null) : base(message, instruction, title, details, inner, owner)
        {
        }
    }
}
