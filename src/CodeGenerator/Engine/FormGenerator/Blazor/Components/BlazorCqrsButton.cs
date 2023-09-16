﻿using System.CodeDom;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration.Models;
using Library.Helpers.CodeGen;
using Library.Validations;

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
        ButtonType type = ButtonType.FormButton,
        string? prefix = null)
        : base("button", id, name, body, prefix)
    {
        this.Type = type;
        this.OnClick = this.ParseOnClickEvent(onClick);
        this.SetCssClasses();
    }

    public TAction? Action { get; set; }
    private string? ParseOnClickEvent(string? onClick) => 
        (onClick, this.Name, this.Id) switch
    {
        (not null, _, _) => onClick,
        (_, not null, _) => this.Name,
        (_, _, not null) => this.Id,
        _ => null
    };

    /// <summary>
    /// Gets the bootstrap button type class.
    /// </summary>
    /// <value>The bootstrap button type class.</value>
    public string BootstrapButtonTypeClass
    {
        get
        {
            string result;
            if (this.IsDefaultButton)
            {
                result = "btn-primary";
            }
            else if (this.IsCancelButton)
            {
                result = "btn-secondary";
            }
            else
            {
                result = "btn-success";
            }

            return result;
        }
    }

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
    public virtual string? OnClick { get => this.GetAttribute("onClick"); set => this.SetAttribute("onClick", this.ParseOnClickEvent(value)); }

    public ButtonType Type { get; set; }

    public IEnumerable<GenerateCodeTypeMemberResult> GenerateTypeMembers(GenerateCodesParameters arguments)
    {
        if (this.OnClick.IsNullOrEmpty() || this.Action is not null)
        {
            return Enumerable.Empty<GenerateCodeTypeMemberResult>();
        }

        var main = CodeDomHelper.NewMethod(this.OnClick, accessModifiers: MemberAttributes.Private);
        return EnumerableHelper.ToEnumerable(new GenerateCodeTypeMemberResult(main, null));
    }

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
    ButtonType type = ButtonType.FormButton,
    string? prefix = null) : BlazorButtonBase<BlazorCqrsButton, ISegregationAction>(id, name, onClick, body, type, prefix), IHasSegregationAction
{
    public override string? OnClick
    {
        get => this.GetAttribute("onclick", isBlazorAttribute: true);
        set => this.SetAttribute("onclick", value, isBlazorAttribute: true);
    }

    public IEnumerable<GenerateCodeTypeMemberResult> GenerateActionCodes()
    {
        if (this.Action is null)
        {
            yield break;
        }
        Check.MutBeNotNull(this.Action.Segregation);
        var calleeName = this.Action.Name;
        var cqrsCommandType = TypePath.New(this.Action.Segregation.Name);
        var cqrsResultType = this.Action.Segregation.Result?.Type ?? TypePath.New<object>();
        var dataContextValidatorName = $"ValidateForm";
        var dataContextValidatorMethod = CodeDomHelper.NewMethod($"{dataContextValidatorName}", accessModifiers: MemberAttributes.Private | MemberAttributes.Final);

        yield return new(dataContextValidatorMethod, null);
        const string INDENT = "    ";
        switch (this.Action.Segregation)
        {
            case IQueryCqrsSegregation query:
                var queryBody = CodeDomHelper.NewMethod(this.OnClick.ArgumentNotNull(nameof(this.OnClick)),
                    $@"{INDENT.Repeat(3)}this.{dataContextValidatorName}();
{INDENT.Repeat(3)}var dto = this.DataContext;
{INDENT.Repeat(3)}On{calleeName}Calling(cqrs);
{INDENT.Repeat(3)}var cqResult = await this._queryProcessor.ExecuteAsync(cqrs);
{INDENT.Repeat(3)}On{calleeName}Called(cqrs, cqResult);", returnType: "async void");
                var queryCalling = CodeDomHelper.NewMethod(
                    $"On{calleeName}Calling({query.Parameter?.Type ?? "System.Object"} parameter)"
                    , accessModifiers: MemberAttributes.Private | MemberAttributes.Final);
                var queryCalled = CodeDomHelper.NewMethod(
                    $"On{calleeName}Called({query.Parameter?.Type} parameter, {query.Result?.Type} result)"
                    , accessModifiers: MemberAttributes.Private | MemberAttributes.Final);
                yield return new(queryCalling, null);
                yield return new(null, queryBody);
                yield return new(queryCalled, null);
                break;

            case ICommandCqrsSegregation command:
                var commandBody = CodeDomHelper.NewMethod(this.OnClick.ArgumentNotNull(nameof(this.OnClick)),
                    $@"{INDENT.Repeat(3)}this.{dataContextValidatorName}();
{INDENT.Repeat(3)}var dto = this.DataContext;
{INDENT.Repeat(3)}var cqrs = new {cqrsCommandType}(dto);
{INDENT.Repeat(3)}On{calleeName}Calling(cqrs);

{INDENT.Repeat(3)}var cqResult = await this._commandProcessor.ExecuteAsync<{cqrsCommandType},{cqrsResultType}>(cqrs);

{INDENT.Repeat(3)}On{calleeName}Called(cqrs, cqResult);", returnType: "async void");
                var commandCalling = CodeDomHelper.NewMethod(
                    $"On{calleeName}Calling"
                    , arguments: new MethodArgument[] {
                        new (command.Parameter?.Type ?? "System.Object", "parameter")
                        }
                    , accessModifiers: MemberAttributes.Private | MemberAttributes.Final);
                var commandCalled = CodeDomHelper.NewMethod(
                    $"On{calleeName}Called"
                    , arguments: new MethodArgument[] {
                        new (command.Parameter?.Type ?? "System.Object", "parameter"),
                        new (command.Result?.Type ?? "System.Object", "result")
                        }
                    , accessModifiers: MemberAttributes.Private | MemberAttributes.Final);
                yield return new(commandCalling, null);
                yield return new(null, commandBody);
                yield return new(commandCalled, null);
                break;

            default:
                throw new NotSupportedException();
        }
    }

    public BlazorCqrsButton SetAction(string name, ICqrsSegregation segregation) =>
        this.Fluent(() => this.Action = new CqrsAction(name, segregation));

    private void SetCssClasses()
    {
        _ = this.CssClasses.Remove("btn");
        _ = this.CssClasses.Remove("btn-primary");
        _ = this.CssClasses.Remove("btn-secondary");
        _ = this.CssClasses.Remove("btn-success");
        this.CssClasses.Add("btn");
        this.CssClasses.Add(this.BootstrapButtonTypeClass);
    }

    public enum ButtonType
    {
        RowButton,
        FormButton,
        Reset
    }
}

public sealed class BlazorCustomButton
    (
    string? id = null,
    string? name = null,
    string? onClick = null,
    string? body = null,
    ButtonType type = ButtonType.FormButton,
    string? prefix = null) : BlazorButtonBase<BlazorCustomButton, ICustomAction>(id, name, onClick, body, type, prefix), IHasCustomAction
{
    public IEnumerable<GenerateCodeTypeMemberResult>? GenerateActionCodes()
    {
        if (this.Action is null)
        {
            yield break;
        }
        Check.MutBeNotNull(this.Action.CodeStatement);

        var dataContextValidatorMethod = CodeDomHelper.NewMethod("ValidateForm", accessModifiers: MemberAttributes.Private | MemberAttributes.Final);
        yield return new(dataContextValidatorMethod, null);

        var queryBody = CodeDomHelper.NewMethod(this.OnClick.ArgumentNotNull(nameof(this.OnClick)), this.Action.CodeStatement, returnType: "void");
        yield return new(null, queryBody);
    }

    public BlazorCustomButton SetAction(string name, string? codeStatement) =>
        this.Fluent(() => this.Action = new CustomAction(name, codeStatement));
}

public enum ButtonType
{
    RowButton,
    FormButton,
    Reset
}