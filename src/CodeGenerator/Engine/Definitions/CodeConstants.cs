using System.CodeDom;

namespace HanyCo.Infra.CodeGeneration.Definitions;

[Immutable]
[Fluent]
public static class CodeConstants
{
    public const MemberAttributes DEFAULT_ACCESS_MODIFIER = MemberAttributes.Private | MemberAttributes.Final;
    public const string INDENT = "    ";

    public static string DefaultMethodBody => "throw new NotImplementedException();";
    public static string DefaultTaskMethodBody => "await Task.CompletedTask;";
    public static string InitializedAsyncMethodBody => $"await this.OnPageInitializedAsync();";
    public static string Keyword_AddToOnInitializedAsync => "OnLoad";

    public static string QueryButton_CalledQueryMethodName(string segregation, string? cqrsParameterType, string? cqrsResultType) =>
        $"On{segregation}Called({cqrsParameterType} parameter, {cqrsResultType} result)";

    public static string QueryButton_CallingQueryMethodName(string segregation, string? queryParameterType) =>
        $"On{segregation}Calling({queryParameterType ?? "System.Object"} parameter)";
}