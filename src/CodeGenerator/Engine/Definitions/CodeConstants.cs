using System.CodeDom;

namespace HanyCo.Infra.CodeGeneration.Definitions;

[Immutable]
[Fluent]
public static class CodeConstants
{
    public const MemberAttributes DEFAULT_ACCESS_MODIFIER = MemberAttributes.Private | MemberAttributes.Final;
    public const string INDENT = "    ";

    public static string DefaultTaskMethodBody => "return Task.CompletedTask;";
    public static string InitializedAsyncMethodBody => $"await this.OnPageInitializedAsync();";
    public static string Keyword_AddToOnInitializedAsync => "OnLoad";

    public static string Converter_Convert_MethodName(string dstClassName) =>
        $"To{dstClassName}";

    public static string Converter_ConvertEnumerable_MethodBody(string srcClassName, string dstClassName, string argName) =>
        $"return {argName}.Select({Converter_Convert_MethodName(dstClassName)});";

    public static string Converter_ConvertSingle_MethodBody(string srcClassName, string dstClassName, string argName, IEnumerable<string?> propNames) =>
        new StringBuilder()
            .AppendLine($"var result = new {dstClassName}")
            .AppendLine($"{{")
            .AppendAllLines(propNames, propName => $"{propName} = {argName}.{propName},")
            .AppendLine($"}};")
            .AppendLine($"return result;")
            .ToString();

    public static string QueryButton_CalledQueryMethodName(string segregation, string? cqrsParameterType, string? cqrsResultType) =>
        $"On{segregation}Called({cqrsParameterType} parameter, {cqrsResultType} result)";

    public static string QueryButton_CallingQueryMethodName(string segregation, string? queryParameterType) =>
        $"On{segregation}Calling({queryParameterType ?? "System.Object"} parameter)";
}