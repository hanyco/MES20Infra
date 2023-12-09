﻿using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration.Models;
using Library.Helpers.CodeGen;

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
        : base("Button", id, name, body, prefix)
    {
        if (type != ButtonType.None)
        {
            this.Type = type.ToString().ToLower();
        }
        else
        {
            this.Type = "";
        }

        this.SetCssClasses();
        this.OnClick = onClick;
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

    public virtual string? OnClick
    {
        get => this.GetAttribute("onclick", isBlazorAttribute: true);
        set
        {
            if (!value.IsNullOrEmpty())
            {
                _ = this.SetAttribute("onclick", value, isBlazorAttribute: true);
            }
            else
            {
                _ = this.Attributes.Remove("onclick");
                _ = this.BlazorAttributes.Remove("onclick");
            }
        }
    }

    public string Type { get; }

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