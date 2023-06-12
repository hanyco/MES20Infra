using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Library.DesignPatterns.Markers;
using Library.Logging;

using Xunit.Abstractions;

namespace InfraTestProject;

[Fluent]
public interface IUnitTestLogger : ILogger
{
    public static IUnitTestLogger New(ITestOutputHelper output)
        => new UnitTestLogger(output);
}

[Immutable]
internal sealed class UnitTestLogger : IUnitTestLogger
{
    private readonly ITestOutputHelper _output;

    public UnitTestLogger(ITestOutputHelper output)
        => this._output = output;

    public bool IsEnabled { get; set; }

    public LogLevel LogLevel { get; set; }

    public void Log([DisallowNull] object message, LogLevel level = LogLevel.Info, [CallerMemberName] object? sender = null, DateTime? time = null, string? stackTrace = null, string? format = LogFormat.SHORT_TIME)
        => this._output.WriteLine(LoggingHelper.Reformat(new LogRecord(message, level, sender, time, stackTrace, format)));
}