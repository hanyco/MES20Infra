﻿using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.Helpers.CodeGen;
using Library.Validations;

using static HanyCo.Infra.CodeGeneration.Definitions.CodeConstants;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public abstract class BlazorButtonBase<TSelf, TAction> : HtmlElementBase<TSelf>, IHtmlElement, IBlazorComponent, ISupportsBehindCodeMember
    where TSelf : BlazorButtonBase<TSelf, TAction>
{
    private bool _isCancelButton;
    private bool _isDefaultButton;

    protected BlazorButtonBase(
        string? id = null,
        string? name = null,
        string? onClick = null,
        string? body = null,
        ButtonType type = ButtonType.None,
        string? prefix = null)
        : base("button", id, name, body, prefix)
    {
        //this.Type = type;
        this.OnClick = this.ParseOnClickEvent(onClick);
        this.SetCssClasses();
    }

    public TAction? Action { get; set; }

    /// <summary>
    /// Gets the bootstrap button type class.
    /// </summary>
    /// <value>The bootstrap button type class.</value>
    public string BootstrapButtonTypeClass => this.IsDefaultButton ? "btn-primary" : this.IsCancelButton ? "btn-secondary" : "btn-success";

    public bool IsCancelButton
    {
        get => this._isCancelButton;
        set
        {
            this._isCancelButton = value;
            this.SetCssClasses();
        }
    }

    public bool IsDefaultButton
    {
        get => this._isDefaultButton;
        set
        {
            this._isDefaultButton = value;
            this.SetCssClasses();
        }
    }

    public string? NameSpace { get; }

    public virtual string? OnClick { get => this.GetAttribute("@onclick"); set => this.SetAttribute("@onclick", this.ParseOnClickEvent(value)); }

    //public ButtonType Type { get; set; }

    public IEnumerable<CodeTypeMembers> GenerateTypeMembers(GenerateCodesParameters arguments)
    {
        if (this.OnClick.IsNullOrEmpty() || this.Action is not null)
        {
            return Enumerable.Empty<CodeTypeMembers>();
        }

        var main = CodeDomHelper.NewMethod(this.OnClick, accessModifiers: DEFAULT_ACCESS_MODIFIER);
        return EnumerableHelper.Iterate(new CodeTypeMembers(main, null));
    }

    private string? ParseOnClickEvent(string? onClick) =>
        (onClick, this.Name, this.Id) switch
        {
            (not null, _, _) => onClick,
            (_, not null, _) => this.Name,
            (_, _, not null) => this.Id,
            _ => null
        };

    private void SetCssClasses()
    {
        _ = this.CssClasses.Remove("btn");
        _ = this.CssClasses.Remove("btn-primary");
        _ = this.CssClasses.Remove("btn-secondary");
        _ = this.CssClasses.Remove("btn-success");
        this.CssClasses.Add("btn");
        this.CssClasses.Add(this.BootstrapButtonTypeClass);
    }
}

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

    public override string? OnClick
    {
        get => this.GetAttribute("onclick", isBlazorAttribute: true);
        set => this.SetAttribute("onclick", value, isBlazorAttribute: true);
    }

    public IEnumerable<CodeTypeMembers> GenerateCodeTypeMembers()
    {
        if (this.Action is null)
        {
            yield break;
        }
        Check.MutBeNotNull(this.Action.Segregation);
        var calleeName = this.Action.Name;
        _ = TypePath.New(this.Action.Segregation.Name);
        var cqrsParamsType = this.Action.Segregation.Parameter?.Type ?? TypePath.New<object>();
        var cqrsResultType = this.Action.Segregation.Result?.Type ?? TypePath.New<object>();
        var dataContextValidatorName = $"ValidateForm";
        var dataContextValidatorMethod = CodeDomHelper.NewMethod($"{dataContextValidatorName}", accessModifiers: DEFAULT_ACCESS_MODIFIER);

        yield return new(dataContextValidatorMethod, null);
        switch (this.Action.Segregation)
        {
            case IQueryCqrsSegregation query:
                var queryBody = CodeDomHelper.NewMethod(this.OnClick.ArgumentNotNull(nameof(this.OnClick))
                    , QueryButton_CallQueryMethodBody(dataContextValidatorName, cqrsParamsType.FullPath, calleeName)
                    , returnType: this._onClickReturnType ?? "async void");
                var queryParameterType = query.Parameter?.Type;
                var queryCalling = CodeDomHelper.NewMethod(
                    QueryButton_CallingQueryMethodName(calleeName, cqrsParamsType.FullPath)
                    , accessModifiers: DEFAULT_ACCESS_MODIFIER);
                var queryResultType = query.Result?.Type;
                var queryCalled = CodeDomHelper.NewMethod(
                    QueryButton_CalledQueryMethodName(calleeName, queryParameterType, queryResultType)
                    , accessModifiers: DEFAULT_ACCESS_MODIFIER);
                yield return new(queryCalling, null);
                yield return new(null, queryBody);
                yield return new(queryCalled, null);
                break;

            case ICommandCqrsSegregation:
                var commandBody = CodeDomHelper.NewMethod(
                    this.OnClick.ArgumentNotNull(nameof(this.OnClick)),
                    CommandButton_CallCommandMethodBody(dataContextValidatorName, cqrsParamsType.FullPath, cqrsResultType.FullPath, calleeName),
                    returnType: "async void");
                var commandCalling = CodeDomHelper.NewMethod(
                    $"On{calleeName}Calling",
                    accessModifiers: DEFAULT_ACCESS_MODIFIER,
                    arguments: [($"{cqrsParamsType.FullPath ?? "System.Object"}", "parameter")],
                    isPartial: true);
                var commandCalled = CodeDomHelper.NewMethod(
                    $"On{calleeName}Called",
                    accessModifiers: DEFAULT_ACCESS_MODIFIER,
                    arguments: [($"{cqrsParamsType.FullPath ?? "System.Object"}", "parameter"), ($"{cqrsResultType?.FullPath ?? "System.Object"}", "result")],
                    isPartial: true);
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

public sealed class BlazorCustomButton(
    string? id = null,
    string? name = null,
    string? onClick = null,
    string? body = null,
    ButtonType type = ButtonType.None,
    string? prefix = null) : BlazorButtonBase<BlazorCustomButton, ICustomAction>(id, name, onClick, body, type, prefix), IHasCustomAction
{
    public string? OnClickReturnType { get; set; }

    public IEnumerable<CodeTypeMembers>? GenerateCodeTypeMembers()
    {
        if (this.Action is null)
        {
            yield break;
        }
        if (!this.Action.CodeStatement.IsNullOrEmpty() && !this.OnClick.IsNullOrEmpty())
        {
            var body = this.Action.CodeStatement;
            var returnValue = this.OnClickReturnType == "void" ? null : this.OnClickReturnType;
            var method = CodeDomHelper.NewMethod(this.OnClick, body, returnType: returnValue);
            yield return body.IsNullOrEmpty() ? new(method, null) : new(null, method);
        }
    }

    public BlazorCustomButton SetAction(string name, string? codeStatement) =>
        this.Fluent(() => this.Action = new CustomAction(name, codeStatement));
}

//public enum ButtonType
//{
//    RowButton,
//    FormButton,
//    Reset
//}

public enum ButtonType
{
    None,
    Button,
    Submit,
    Reset
}