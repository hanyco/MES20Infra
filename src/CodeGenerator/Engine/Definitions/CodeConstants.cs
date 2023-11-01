using System.CodeDom;

using Library.CodeGeneration;
using Library.Helpers.CodeGen;

namespace HanyCo.Infra.CodeGeneration.Definitions;

[Immutable]
[Fluent]
public static class CodeConstants
{
    public const MemberAttributes DEFAULT_ACCESS_MODIFIER = MemberAttributes.Private | MemberAttributes.Final;
    public const string INDENT = "    ";

    public static string Component_OnInitializedAsync_MethodBody(string? onInitializedAsyncAdditionalBody)
    {
        var result = onInitializedAsyncAdditionalBody
            .SplitMerge(mergeSeparator: $"{Environment.NewLine}{INDENT.Repeat(3)}")
            .Add(Environment.NewLine)
            .Add($"// Call developer's method.")
            .Add(Environment.NewLine)
            .Add($"await this.OnLoadAsync();");
        return result;
    }

    public static string Converter_Convert_MethodName(string dstClassName)
        => $"To{dstClassName}";

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

    public static string DefaultTaskMethodBody() =>
        "return Task.CompletedTask;";

    public static string GetAll_OnCalledMethodName(string entityName) =>
        $"OnCalledGetAll{entityName}Query(Dtos.GetAll{entityName}QueryParams cqParams, Queries.GetAll{entityName}QueryResult cqResult)";

    public static string GetAll_OnCallingMethodName(string entityName) =>
            $"OnCallingGetAll{entityName}Query(Dtos.GetAll{entityName}QueryParams cqParams)";

    public static string InitializedAsyncMethodBody() =>
        $"await this.OnPageInitializedAsync();";

    public static string InstanceDataContextProperty(string? name) =>
        $"this.DataContext.{name}";

    public static string Keyword_AddToOnInitializedAsync() =>
        "OnLoad";

    public static string QueryButton_CalledQueryMethodName(string segregation, string? cqrsParameterType, string? cqrsResultType) =>
        $"On{segregation}Called({cqrsParameterType} parameter, {cqrsResultType} result)";

    public static string QueryButton_CallingQueryMethodName(string segregation, string? queryParameterType) =>
        $"On{segregation}Calling({queryParameterType ?? "System.Object"} parameter)";

    public static string QueryButton_CallQueryMethodBody(string dataContextValidatorName, string cqrsParamsType, string segregation) =>
        new StringBuilder()
            .AppendLine($"this.{dataContextValidatorName}()")
            .AppendLine($"var dto = this.DataContext;")
            .AppendLine($"var cqrs = new {cqrsParamsType}(dto);")
            .AppendLine($"On{segregation}Calling(cqrs);")
            .AppendLine($"var cqResult = await this._queryProcessor.ExecuteAsync(cqrs);")
            .AppendLine($"On{segregation}Called(cqrs, cqResult);")
            .ToString();

    public static string WrapInClass(string className, bool isPartial, MemberAttributes accessModifier, params string[] members)
    {
        return new StringBuilder()
        .AppendLine($"public {(isPartial ? "partial" : "")} class {className}")
        //.Append(INDENT.Repeat(0))
        //.Append(accessModifier.Contains(MemberAttributes.Private) ? "private" : "")
        //.Append(accessModifier.Contains(MemberAttributes.Family) ? "internal" : "")
        //.Append(accessModifier.Contains(MemberAttributes.Public) ? "public" : "")
        //.Append(accessModifier.Contains(MemberAttributes.Final) ? "sealed" : "")
        //.Append(accessModifier.Contains(MemberAttributes.Abstract) ? "abstract" : "")
        .AppendLine($"{INDENT.Repeat(0)}{{")
        .AppendAllLines(members.Merge(Environment.NewLine).Split(Environment.NewLine))
        .AppendLine($"{INDENT.Repeat(0)}}}")
        .ToString();
    }
}