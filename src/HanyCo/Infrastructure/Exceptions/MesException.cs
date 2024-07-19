using System.Diagnostics;

namespace HanyCo.Infra.Exceptions
{
    public sealed class MesException : MesExceptionBase
    {
        public MesException()
        {
        }

        public MesException(string message) : base(message)
        {
        }

        public MesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MesException(string message, string? instruction = null, string? title = null, string? details = null, Exception? inner = null, object? owner = null) : base(message, instruction, title, details, inner, owner)
        {
        }

        [StackTraceHidden]
        [DebuggerStepThrough]
        public static void Throw(string message) =>
            throw new MesException(message);
    }
}
