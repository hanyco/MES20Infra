using System.CodeDom;

using Library.Helpers.CodeGen;

namespace HanyCo.Infra.CodeGeneration.Definitions;

[Immutable]
[Fluent]
public static class CodeConstants
{
    public const MemberAttributes DEFAULT_ACCESS_MODIFIER = MemberAttributes.Private | MemberAttributes.Final;
    public const string INDENT = "    ";

    public static string CommandButton_CallCommandMethodBody(string dataContextValidatorName, string cqrsParamsType, string cqrsResultType, string segregation) =>
        new StringBuilder()
            .AppendLine($"{INDENT.Repeat(3)}this.{dataContextValidatorName}();")
            .AppendLine($"{INDENT.Repeat(3)}var dto = this.DataContext;")
            .AppendLine($"{INDENT.Repeat(3)}var cqParams = new {cqrsParamsType}(dto);")
            .AppendLine($"{INDENT.Repeat(3)}On{segregation}Calling(cqParams);")
            .AppendLine($"{INDENT.Repeat(3)}var cqResult = await this._commandProcessor.ExecuteAsync<{cqrsParamsType},{cqrsResultType}>(cqParams);")
            .AppendLine($"{INDENT.Repeat(3)}On{segregation}Called(cqParams, cqResult);")
            .ToString();

    public static string Component_OnInitializedAsync_MethodBody(string? onInitializedAsyncAdditionalBody)
    {
        var result = onInitializedAsyncAdditionalBody
            .Remove(INDENT.Repeat(3))
            .SplitMerge(mergeSeparator: $"{Environment.NewLine}{INDENT.Repeat(3)}")
            .Add(Environment.NewLine)
            .Add($"{INDENT.Repeat(3)}// Call developer's method.")
            .Add(Environment.NewLine)
            .Add($"{INDENT.Repeat(3)}await this.OnLoadAsync();")
            .AddStart(INDENT.Repeat(3));
        return result;
    }

    public static string Converter_Convert_MethodName(string dstClassName)
        => $"To{dstClassName}";

    public static string Converter_ConvertEnumerable_MethodBody(string srcClassName, string dstClassName, string argName) =>
            //$"{INDENT.Repeat(1)}public static IEnumerable<{dstClassName}?> {Converter_Convert_MethodName(dstClassName)}(this IEnumerable<{srcClassName}?> {argName}) =>{Environment.NewLine}{INDENT.Repeat(2)}{argName}.Select(ToDbEntity);";
            $"{INDENT.Repeat(2)}{argName}.Select(ToDbEntity);";

    public static string Converter_ConvertSingle_MethodBody(string srcClassName, string dstClassName, string argName, IEnumerable<string?> propNames) =>
        new StringBuilder()
            //.AppendLine($"{INDENT.Repeat(1)}public static {dstClassName} {Converter_Convert_MethodName(dstClassName)}(this {srcClassName} {argName})")
            //.AppendLine($"{INDENT.Repeat(1)}{{")
            .AppendLine($"{INDENT.Repeat(2)}var result = new {dstClassName}")
            .AppendLine($"{INDENT.Repeat(2)}{{")
            .AppendAllLines(propNames, propName => $"{INDENT.Repeat(3)}{propName} = {argName}.{propName}")
            .AppendLine($"{INDENT.Repeat(2)}}};")
            .AppendLine($"{INDENT.Repeat(2)}return result;")
            //.AppendLine($"{INDENT.Repeat(1)}}}")
            .ToString();

    public static string DefaultTaskMethodBody() =>
        $"{INDENT.Repeat(3)}return Task.CompletedTask;";

    public static string GetAll_CallMethodBody(string entityName) =>
        new StringBuilder()
            .AppendLine($"// Setup segregation parameters")
            .AppendLine($"var paramsParams = new Dtos.GetAll{entityName}Params();")
            .AppendLine($"var cqParams = new Queries.GetAll{entityName}QueryParameter(paramsParams);")
            .AppendLine($"")
            .AppendLine($"// Let the developer know what's going on.")
            .AppendLine($"cqParams = OnCallingGetAll{entityName}Query(cqParams);")
            .AppendLine($"")
            .AppendLine($"// Invoke the query handler to retrieve all entities")
            .AppendLine($"var cqResult = await this._queryProcessor.ExecuteAsync(cqParams);")
            .AppendLine($"")
            .AppendLine($"// Let's inform the developer about the result.")
            .AppendLine($"cqResult = OnCalledGetAll{entityName}Query(cqParams, cqResult);")
            .AppendLine($"")
            .AppendLine($"// Now, set the data context.")
            //TODO: `ToDo()` method must be written.
            .AppendLine($"this.DataContext = cqResult.Result.To{StringHelper.Singularize(entityName)}Dto();")
            .ToString();

    public static string GetAll_OnCalledMethodName(string entityName) =>
        $"OnCalledGetAll{entityName}Query(Queries.GetAll{entityName}QueryParameter cqParams, Queries.GetAll{entityName}QueryResult cqResult)";

    public static string GetAll_OnCallingMethodName(string entityName) =>
            $"OnCallingGetAll{entityName}Query(Queries.GetAll{entityName}QueryParameter cqParams)";

    public static string InitializedAsyncMethodBody() =>
        $"{INDENT}await this.OnPageInitializedAsync();";

    public static string InstanceDataContextProperty(string? name) =>
        $"this.DataContext.{name}";

    public static string Keyword_AddToOnInitializedAsync() =>
        "OnLoad";

    public static string OnCalledCqrsMethodName(string segregation, string? cqrsParameterType, string? cqrsResultType) =>
        $"On{segregation}Called({cqrsParameterType} parameter, {cqrsResultType} result)";

    public static string OnCallingCqrsMethodName(string segregation, string? cqrsParameterType = null) =>
        $"On{segregation}Calling({cqrsParameterType ?? "System.Object"} parameter)";

    public static string QueryButton_CalledQueryMethodName(string segregation, string? cqrsParameterType, string? cqrsResultType) =>
        $"On{segregation}Called({cqrsParameterType} parameter, {cqrsResultType} result)";

    public static string QueryButton_CallingQueryMethodName(string segregation, string? queryParameterType) =>
        $"On{segregation}Calling({queryParameterType ?? "System.Object"} parameter)";

    public static string QueryButton_CallQueryMethodBody(string dataContextValidatorName, string cqrsParamsType, string segregation) =>
        new StringBuilder()
            .AppendLine($"{INDENT.Repeat(3)}this.{dataContextValidatorName}()")
            .AppendLine($"{INDENT.Repeat(3)}var dto = this.DataContext;")
            .AppendLine($"{INDENT.Repeat(3)}var cqrs = new {cqrsParamsType}(dto);")
            .AppendLine($"{INDENT.Repeat(3)}On{segregation}Calling(cqrs);")
            .AppendLine($"{INDENT.Repeat(3)}var cqResult = await this._queryProcessor.ExecuteAsync(cqrs);")
            .AppendLine($"{INDENT.Repeat(3)}On{segregation}Called(cqrs, cqResult);")
            .ToString();

    public static string WrapInClass(string className, bool isPartial, MemberAttributes accessModifier, params string[] members)
    {
        return new StringBuilder()
        .AppendLine($"{INDENT.Repeat(0)}public {(isPartial ? "partial" : "")} class {className}")
        //.Append(INDENT.Repeat(0))
        //.Append(accessModifier.Contains(MemberAttributes.Private) ? "private" : "")
        //.Append(accessModifier.Contains(MemberAttributes.Family) ? "internal" : "")
        //.Append(accessModifier.Contains(MemberAttributes.Public) ? "public" : "")
        //.Append(accessModifier.Contains(MemberAttributes.Final) ? "sealed" : "")
        //.Append(accessModifier.Contains(MemberAttributes.Abstract) ? "abstract" : "")
        .AppendLine($"{INDENT.Repeat(0)}{{")
        .AppendAllLines(members.Merge(Environment.NewLine).Split(Environment.NewLine)
            , line => line.StartsWith(INDENT) ? line : $"{INDENT.Repeat(1)}{line}")
        .AppendLine($"{INDENT.Repeat(0)}}}")
        .ToString();
    }
}