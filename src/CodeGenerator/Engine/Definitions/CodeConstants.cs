﻿using System.CodeDom;

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

    public static string CommandButton_CalledCommandMethodName(string segregation, string? cqrsParameterType, string? cqrsResultType) =>
            $"On{segregation}Called({cqrsParameterType} parameter, {cqrsResultType} result)";

    public static string CommandButton_CallingCommandMethodName(string segregation, string? cqrsParameterType) =>
            $"On{segregation}Calling({cqrsParameterType ?? "System.Object"} parameter)";

    public static string ConverterToModelClassSource(string dtoName, string dstClassName, string argName, IEnumerable<string?> propNames) =>
        new StringBuilder()
            .AppendLine($"{INDENT.Repeat(0)}public partial class ModelConverter")
            .AppendLine($"{INDENT.Repeat(0)}{{")
            .AppendLine($"{INDENT.Repeat(1)}public static {dstClassName} To{dstClassName}(this {dtoName} {argName})")
            .AppendLine($"{INDENT.Repeat(1)}{{")
            .AppendLine($"{INDENT.Repeat(2)}var result = new {dstClassName}")
            .AppendLine($"{INDENT.Repeat(2)}{{")
            .AppendAllLines(propNames, propName => $"{INDENT.Repeat(3)}{propName} = {argName}.{propName}")
            .AppendLine($"{INDENT.Repeat(2)}}};")
            .AppendLine($"{INDENT.Repeat(2)}return result;")
            .AppendLine($"{INDENT.Repeat(1)}}}")
            .AppendLine($"{INDENT.Repeat(0)}}}")
            .ToString();

    public static string InitializedAsyncMethodBody() =>
            $"{INDENT}await this.OnPageInitializedAsync();";

    public static string InstanceDataContextProperty(string? name) =>
            $"this.DataContext.{name}";

    public static string CallGetAllMethodBody(string entityType, string queryResult)
    {
        return new StringBuilder()
            .AppendLine($@"{INDENT.Repeat(3)}// Create an instance of the GetAllQuery")
            .AppendLine($@"{INDENT.Repeat(3)}var cqParams = new GetAllQuery<{entityType}>();")
            .AppendLine($@"{INDENT.Repeat(3)}")
            .AppendLine($@"{INDENT.Repeat(3)}// Create an instance of the query handler")
            .AppendLine($@"{INDENT.Repeat(3)}var handler = new GetAllQueryHandler<{entityType}>();")
            .AppendLine($@"{INDENT.Repeat(3)}")
            .AppendLine($@"{INDENT.Repeat(3)}// Invoke the query handler to retrieve all customers")
            .AppendLine($@"{INDENT.Repeat(3)}var cqResult = handler.Handle(cqParams);")
            .AppendLine($@"{INDENT.Repeat(3)}")
            .AppendLine($@"{INDENT.Repeat(3)}this.DataContext = cqResult;")
            .ToString();

    }

    public static string QueryButton_CalledQueryMethodName(string segregation, string? cqrsParameterType, string? cqrsResultType) =>
        $"On{segregation}Called({cqrsParameterType} parameter, {cqrsResultType} result)";

    public static string QueryButton_CallingQueryMethodName(string segregation, string? queryParameterType) =>
        $"On{segregation}Calling({queryParameterType ?? "System.Object"} parameter)";

    public static string QueryButton_CallQueryMethodBody(string dataContextValidatorName, string cqrsParamsType, string segregation) =>
        new StringBuilder()
            .AppendLine($@"{INDENT.Repeat(3)}this.{dataContextValidatorName}()")
            .AppendLine($@"{INDENT.Repeat(3)}var dto = this.DataContext;")
            .AppendLine($@"{INDENT.Repeat(3)}var cqrs = new {cqrsParamsType}(dto);")
            .AppendLine($@"{INDENT.Repeat(3)}On{segregation}Calling(cqrs);")
            .AppendLine($@"{INDENT.Repeat(3)}var cqResult = await this._queryProcessor.ExecuteAsync(cqrs);")
            .AppendLine($@"{INDENT.Repeat(3)}On{segregation}Called(cqrs, cqResult);")
            .ToString();
}