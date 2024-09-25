using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration;
using Library.CodeGeneration.v2.Back;
using Library.Helpers.CodeGen;
using Library.Validations;

using static HanyCo.Infra.CodeGeneration.Definitions.CodeConstants;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorCqrsButton(
    string? id = null,
    string? name = null,
    string? onClick = null,
    string? body = null,
    ButtonType type = ButtonType.None,
    string? prefix = null,
    string? onClickReturnType = null) : BlazorButtonBase<BlazorCqrsButton, ISegregationAction>(id, name, onClick, body, type, prefix), IHasSegregationAction
{
    private readonly string? _onClickReturnType = onClickReturnType;

    public IEnumerable<ClassMembers> GenerateCodeTypeMembers()
    {
        if (this.Action is null)
        {
            yield break;
        }
        Check.MustBeNotNull(this.Action.Segregation);
        var calleeName = this.Action.Name;
        var dataContextValidatorName = "ValidateForm";
        var dataContextValidatorMethod = new Method(dataContextValidatorName) { AccessModifier = AccessModifier.Private };

        yield return new(dataContextValidatorMethod, null);
        var cqrsParamsType = this.Action.Segregation.Parameter?.Type ?? TypePath.New<object>();
        switch (this.Action.Segregation)
        {
            case IQueryCqrsSegregation query:
                var queryBody = new Method(this.OnClick.ArgumentNotNull())
                {
                    Body = QueryButton_CallQueryMethodBody(dataContextValidatorName, cqrsParamsType.FullPath, calleeName),
                    ReturnType = this._onClickReturnType ?? "async void"

                };
                var queryParameterType = query.Parameter?.Type;
                var queryCalling = new Method(QueryButton_CallingQueryMethodName(calleeName, cqrsParamsType.FullPath)) { AccessModifier = AccessModifier.Private };
                var queryResultType = query.Result?.Type;
                var queryCalled = new Method(QueryButton_CalledQueryMethodName(calleeName, queryParameterType, queryResultType))
                {
                    AccessModifier = AccessModifier.Private
                };
                yield return new(queryCalling, null);
                yield return new(null, queryBody);
                yield return new(queryCalled, null);
                break;

            case ICommandCqrsSegregation:
                var cqrsResultType = this.Action.Segregation.Result?.Type ?? TypePath.New<object>();
                var commandBody = new Method(this.OnClick.ArgumentNotNull())
                {
                    Body = CommandButton_CallCommandMethodBody(dataContextValidatorName, cqrsParamsType.FullPath, cqrsResultType.FullPath, calleeName),
                    ReturnType = "async void"
                };
                var commandCalling = new Method($"On{calleeName}Calling")
                {
                    AccessModifier = AccessModifier.Private,
                    InheritanceModifier = InheritanceModifier.Partial
                }.AddArgument($"{cqrsParamsType?.FullPath ?? "System.Object"}", "parameter");
                var commandCalled = new Method($"On{calleeName}Called")
                {
                    AccessModifier = AccessModifier.Private,
                    InheritanceModifier = InheritanceModifier.Partial
                }.AddArgument(($"{cqrsParamsType.FullPath ?? "System.Object"}", "parameter"), ($"{cqrsResultType?.FullPath ?? "System.Object"}", "result"));
                yield return new(commandCalling, null);
                yield return new(null, commandBody);
                yield return new(commandCalled, null);
                break;

            default:
                throw new NotSupportedException();
        }

        static string QueryButton_CallQueryMethodBody(string dataContextValidatorName, string cqrsParamsType, string segregation) =>
            new StringBuilder()
                .AppendLine($"this.{dataContextValidatorName}()")
                .AppendLine($"var dto = this.DataContext;")
                .AppendLine($"var cqrs = new {cqrsParamsType}(dto);")
                .AppendLine($"On{segregation}Calling(cqrs);")
                .AppendLine($"var cqResult = await this._queryProcessor.ExecuteAsync(cqrs);")
                .AppendLine($"On{segregation}Called(cqrs, cqResult);")
                .ToString();
    }

    public BlazorCqrsButton SetAction(string name, ICqrsSegregation segregation) =>
        this.Fluent(() => this.Action = new CqrsAction(name, segregation));

    private static string CommandButton_CallCommandMethodBody(string dataContextValidatorName, string cqrsParamsType, string cqrsResultType, string segregation) =>
        new StringBuilder()
            .AppendLine($"this.{dataContextValidatorName}();")
            .AppendLine($"var dto = this.DataContext;")
            .AppendLine($"var cqParams = new {cqrsParamsType}(dto);")
            .AppendLine($"On{segregation}Calling(cqParams);")
            .AppendLine($"var cqResult = await this._commandProcessor.ExecuteAsync<{cqrsParamsType}, {cqrsResultType}>(cqParams);")
            .AppendLine($"On{segregation}Called(cqParams, cqResult);")
            .ToString();
}