﻿using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Actions;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2.Back;
using Library.Helpers.CodeGen;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;

public sealed class BlazorCustomButton(
    string? id = null,
    string? name = null,
    string? onClick = null,
    string? body = null,
    ButtonType type = ButtonType.None,
    string? prefix = null) : BlazorButtonBase<BlazorCustomButton, ICustomAction>(id, name, onClick, body, type, prefix), IHasCustomAction
{
    public string? OnClickReturnType { get; set; }

    public IEnumerable<ClassMembers>? GenerateCodeTypeMembers()
    {
        if (this.Action is null)
        {
            yield break;
        }
        if (!this.Action.CodeStatement.IsNullOrEmpty() && !this.OnClick.IsNullOrEmpty())
        {
            var body = this.Action.CodeStatement;
            var returnValue = this.OnClickReturnType == "void" ? null : this.OnClickReturnType;
            var method = new Method(this.OnClick) { Body = body, ReturnType = returnValue };
            yield return body.IsNullOrEmpty() ? new(method, null) : new(null, method);
        }
    }

    public BlazorCustomButton SetAction(string name, string? codeStatement) =>
        this.Fluent(() => this.Action = new CustomAction(name, codeStatement));
}