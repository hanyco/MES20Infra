using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.CodeGeneration.Helpers;
using Library.CodeGeneration.Models;
using Library.Coding;
using Library.Helpers.CodeGen;
using Library.Validations;
using System.CodeDom;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;

public class HtmlButton : HtmlElementBase<HtmlButton>, IHtmlElement, IHasHtmlAction
{
    private bool _isCancellButton;
    private bool _isDefaultButton;
    public HtmlButton(
        string? id = null,
        string? name = null,
        string? onClick = null,
        string? body = null,
        ButtonType type = ButtonType.Button,
        string? prefix = null)
        : base("button", id, name, body, prefix)
    {
        this.Type = type;
        this.OnClick = this.ParseOnClickEvent(onClick);// !onClick.IsNullOrEmpty() ? onClick : $"{this.Name ?? this.Id}_Click";
        this.SetCssClasses();
    }

    private string? ParseOnClickEvent(string? onClick) => (onClick, this.Name, this.Id) switch
    {
        (not null, _, _) => onClick,
        (_, not null, _) => this.Name,
        (_, _, not null) => this.Id,
        _ => null
    };
    public IHtmlAction? Action { get; private set; }

    /// <summary>
    /// Gets the bootstrap button type class.
    /// </summary>
    /// <value>
    /// The bootstrap button type class.
    /// </value>
    public string BootstrapButtonTypeClass
    {
        get
        {
            string result;
            if (this.IsDefaultButton)
            {
                result = "btn-primary";
            }
            else if (this.IsCancellButton)
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

    public bool IsCancellButton
    {
        get => this._isCancellButton;
        set
        {
            this._isCancellButton = value;
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

    public virtual string? OnClick { get => this.GetAttribute("onClick"); set => this.SetAttribute("onClick", this.ParseOnClickEvent(value)); }

    public ButtonType Type
    {
        get => this.GetAttribute("type") switch
        {
            "button" => ButtonType.Button,
            "sumbit" => ButtonType.Submit,
            "reset" => ButtonType.Reset,
            _ => throw new NotSupportedException(),
        }; set => _ = this.SetAttribute("type", value.ToString().ToLowerInvariant());
    }

    public IEnumerable<GenerateCodeTypeMemberResult> GenerateActionCodes()
    {
        if (this.Action is null)
        {
            yield break;
        }
        Check.NotNull(this.Action.Segregation);
        var calleeName = this.Action.Name;
        var cqrsCommandType = TypePath.New(this.Action.Segregation.Name);
        var cqrsResultType = this.Action.Segregation.Result?.Type ?? TypePath.New<object>();
        var dataContextValidatorName = $"ValidateForm";
        var dataContextValidatorMethod = CodeDomHelper.NewMethod($"{dataContextValidatorName}", accessModifiers: MemberAttributes.Private | MemberAttributes.Final);

        yield return new(dataContextValidatorMethod, null);
        const string INDENT = "    ";
        switch (this.Action.Segregation)
        {
            case IQueryCqrsSergregation query:
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

            case ICommandCqrsSergregation command:
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

    public HtmlButton SetAction(IHtmlAction action)
        => this.Fluent(() => this.Action = action);

    public HtmlButton SetAction(string name, ICommandCqrsSergregation segregation)
        => this.Fluent(() => this.Action = new HtmlCommandAction(name, segregation));

    public HtmlButton SetAction(string name, IQueryCqrsSergregation segregation)
        => this.Fluent(() => this.Action = new HtmlQueryAction(name, segregation));

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
        Submit,
        Button,
        Reset
    }
}
