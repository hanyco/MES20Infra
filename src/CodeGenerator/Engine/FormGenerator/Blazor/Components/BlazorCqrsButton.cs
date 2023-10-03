using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

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
        ButtonType type = ButtonType.FormButton,
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
        return EnumerableHelper.ToEnumerable(new CodeTypeMembers(main, null));
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
    ButtonType type = ButtonType.FormButton,
    string? prefix = null) : BlazorButtonBase<BlazorCqrsButton, ISegregationAction>(id, name, onClick, body, type, prefix), IHasSegregationAction
{
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
                    , QueryButton_CallQueryMethodBody(dataContextValidatorName, cqrsParamsType, calleeName)
                    , returnType: "async void");
                var queryParameterType = query.Parameter?.Type;
                var queryCalling = CodeDomHelper.NewMethod(
                    QueryButton_CallingQueryMethodName(calleeName, cqrsParamsType)
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
                var commandBody = CodeDomHelper.NewMethod(this.OnClick.ArgumentNotNull(nameof(this.OnClick))
                    , CommandButton_CallCommandMethodBody(dataContextValidatorName, cqrsParamsType, cqrsResultType, calleeName)
                    , returnType: "async void");
                var commandCalling = CodeDomHelper.NewMethod(
                    OnCallingCqrsMethodName(calleeName, cqrsParamsType)
                    , accessModifiers: DEFAULT_ACCESS_MODIFIER);
                var commandCalled = CodeDomHelper.NewMethod(
                    OnCalledCqrsMethodName(calleeName, cqrsParamsType, cqrsResultType)
                    , accessModifiers: DEFAULT_ACCESS_MODIFIER);
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
}

public sealed class BlazorCustomButton(
    string? id = null,
    string? name = null,
    string? onClick = null,
    string? body = null,
    ButtonType type = ButtonType.FormButton,
    string? prefix = null) : BlazorButtonBase<BlazorCustomButton, ICustomAction>(id, name, onClick, body, type, prefix), IHasCustomAction
{
    public IEnumerable<CodeTypeMembers>? GenerateCodeTypeMembers()
    {
        if (this.Action is null)
        {
            yield break;
        }
        Check.MutBeNotNull(this.Action.CodeStatement);

        var dataContextValidatorMethod = CodeDomHelper.NewMethod("ValidateForm", accessModifiers: DEFAULT_ACCESS_MODIFIER);
        yield return new(dataContextValidatorMethod, null);

        var body = this.Action.CodeStatement?.ToString().Split(Environment.NewLine).Merge(INDENT.Repeat(3), false);

        var method = CodeDomHelper.NewMethod(this.OnClick.ArgumentNotNull(nameof(this.OnClick)), body);
        yield return body.IsNullOrEmpty() ? new(method, null) : new(null, method);
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