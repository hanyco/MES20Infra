﻿using System.CodeDom;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.DesignPatterns.Behavioral.Observation;
using Library.Exceptions.Validations;
using Library.Helpers.CodeGen;
using Library.Interfaces;
using Library.Validations;

using static HanyCo.Infra.CodeGeneration.Definitions.CodeConstants;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;

[Fluent]
public abstract class BlazorComponentBase<TBlazorComponent> : IHtmlElement, IParent<IHtmlElement>//, ICodeGenerator,  IComponentCodeUnit
    where TBlazorComponent : BlazorComponentBase<TBlazorComponent>
{
    private static readonly string[] _parameterAttributes = ["Microsoft.AspNetCore.Components.Parameter"];
    private BootstrapPosition? _position;

    protected BlazorComponentBase(in string name) => this.Name = name;

    public IList<MethodActor> Actions { get; } = new List<MethodActor>();
    public HashSet<string> AdditionalUsings { get; } = [];
    public IDictionary<string, string?> Attributes { get; } = new Dictionary<string, string?>();
    Dictionary<string, string?> IHtmlElement.Attributes { get; }
    public IList<IHtmlElement> Children { get; } = new List<IHtmlElement>();
    public TypePath? DataContextType { get; set; }
    public IList<FieldInfo> Fields { get; } = new List<FieldInfo>();
    public virtual string HtmlFileExtension { get; } = "razor";
    public bool IsGrid { get; set; }
    public virtual string MainCodeFileExtension { get; } = "razor.cs";
    public IList<string> MainCodeUsingNameSpaces { get; } = new List<string>();
    public string Name { get; init; }
    public string? NameSpace { get; set; }
    public IList<MethodArgument> Parameters { get; } = new List<MethodArgument>();
    public virtual string PartialCodeFileExtension { get; } = "partial.cs";
    public IList<string> PartialCodeUsingNameSpaces { get; } = new List<string>();
    public BootstrapPosition Position { get => this._position ??= new(); set => this._position = value; }
    public IList<PropertyActor> Properties { get; } = new List<PropertyActor>();

    public Codes GenerateCodes(CodeCategory category, GenerateCodesParameters? arguments = null)
    {
        var args = arguments ?? GenerateCodesParameters.FullCode();
        _ = validate(args);
        var codes = new List<Code>();
        if (args.GenerateUiCode)
        {
            var htmlCode = this.GenerateUiCode(category, args);
            if (htmlCode is { } c)
            {
                codes.Add(c.With(x =>
                {
                    x.props().Category = category;
                    return x;
                }));
            }
        }
        if (args.GenerateMainCode || args.GeneratePartialCode)
        {
            var (mainCs, partialCs) = this.GenerateBehindCode(args);

            if (mainCs is { } c2)
            {
                codes.Add(c2.With(x =>
                {
                    x.props().Category = category;
                    return x;
                }));
            }

            if (partialCs is { } c3)
            {
                codes.Add(c3.With(x =>
                {
                    x.props().Category = category;
                    return x;
                }));
            }
        }

        return codes.ToCodes();

        TBlazorComponent validate(GenerateCodesParameters arguments)
        {
            _ = this.Check(CheckBehavior.ThrowOnFail)
                    .NotNull(() => new ValidationException("Please initialize a new component."))
                    .NotNull(x => x.Name)
                    .NotNull(x => x.NameSpace);
            if (!arguments.GenerateMainCode && !arguments.GeneratePartialCode && !arguments.GenerateUiCode)
            {
                throw new ValidationException("Please select a code generation option at least.", "No code generation option is selected.", owner: this);
            }
            if (this.Children.Count != 0)
            {
                if (EnumerableHelper.FindDuplicates(this.Children).Any())
                {
                    throw new ObjectDuplicateValidationException(nameof(this.Children));
                }
            }
            if (this.Actions.Count != 0)
            {
                if (EnumerableHelper.FindDuplicates(this.Actions).Any())
                {
                    throw new ObjectDuplicateValidationException(nameof(this.Actions));
                }
            }
            if (this.Properties.Count != 0)
            {
                if (EnumerableHelper.FindDuplicates(this.Properties).Any())
                {
                    throw new ObjectDuplicateValidationException(nameof(this.Properties));
                }
            }
            return this.This();
        }
    }

    public Code GenerateUiCode(GenerateCodesParameters? arguments = null) =>
        this.GenerateUiCode(CodeCategory.Component, arguments);

    protected virtual string? GetBaseTypes() =>
        null;

    protected virtual StringBuilder OnGeneratingHtmlCode(StringBuilder codeStringBuilder) =>
        codeStringBuilder;

    protected virtual Code OnGeneratingUiCode(in GenerateCodesParameters? arguments = null)
    {
        var args = arguments ?? GenerateCodesParameters.FullCode();
        var buffer = new StringBuilder();

        _ = this.OnGeneratingHtmlCode(buffer);

        if (args.IsEditForm)
        {
            _ = buffer.AppendLine($"<EditForm {args.EditFormAttributes?.Select(x => $"{x.Key}='{x.Value}' ").Merge().Trim()}>");
            _ = buffer.AppendLine($"{INDENT}<DataAnnotationsValidator />");
            _ = buffer.AppendLine($"{INDENT}<ValidationSummary />");
        }
        if (this.IsGrid)
        {
            _ = generateGridCode(buffer);
        }
        else
        {
            _ = generateDetailsCode(buffer);
        }

        if (args.IsEditForm)
        {
            _ = buffer.AppendLine("</EditForm>");
        }

        var statement = buffer.ToString();
        if (statement.IsNullOrEmpty())
        {
            return Code.Empty;
        }
        var htmlFileName = Path.ChangeExtension($"{this.Name}.tmp", this.HtmlFileExtension);
        return Code.New(this.Name, Languages.BlazorFront, statement, false, htmlFileName);

        StringBuilder generateDetailsCode(StringBuilder sb) =>
            this.Children.GenerateChildrenCode(sb, isEditForm: args.IsEditForm);

        StringBuilder generateGridCode(StringBuilder sb)
        {
            var buttonsCode = this.Actions.OfType<ButtonActor>().Where(x => !x.ShowOnGrid)
                .Select(method =>
                {
                    IUiCodeGenerator result = method.Body.IsNullOrEmpty()
                        ? new BlazorCqrsButton(name: method.Name, body: method.Caption, onClick: method.EventHandlerName, onClickReturnType: returnType(method.ReturnType))
                                .With(x => x.Position.SetCol(1))
                        : new BlazorCustomButton(name: method.Name, body: method.Caption, onClick: method.EventHandlerName) { OnClickReturnType = returnType(method.ReturnType) }
                                .With(x => x.Position.SetCol(1));
                    return result;
                })
                .Select(x => x.GenerateUiCode().Statement).Merge(Environment.NewLine);

            var table = new BlazorTable
            {
                DataContextName = "this.DataContext"
            };
            _ = table.Columns.AddRange(this.Properties.Select(x => new BlazorTableColumn(x.Name, x.Caption!) { BindingName = x.BindingName.ArgumentNotNull() }));
            _ = table.Actions.AddRange(this.Actions.OfType<ButtonActor>().Where(x => x.ShowOnGrid)
                .Select(x => new BlazorTableRowAction(x.Name!, x.Caption!) { OnClick = x.EventHandlerName }));
            var tableCode = table.GenerateUiCode().Statement;

            var result = sb.AppendLine()
                .AppendLine("<div class=\"row\">")
                .AppendLine(buttonsCode)
                .AppendLine("</div>")
                .AppendLine("<div class=\"row\">")
                .AppendLine(tableCode)
                .AppendLine("</div>");
            return result;
        }
    }

    protected virtual void OnInitializingBehindCode(GenerateCodesParameters? arguments)
    {
    }

    protected virtual void OnInitializingUiCode(GenerateCodesParameters? arguments)
    {
    }

    protected TBlazorComponent This()
        => (this as TBlazorComponent)!;

    private static string Component_OnInitializedAsync_MethodBody(in string? onInitializedAsyncAdditionalBody)
    {
        var result = new StringBuilder(onInitializedAsyncAdditionalBody)
            .AppendLine()
            .AppendLine($"// Call developer's method.")
            .AppendLine($"await this.OnLoadAsync();");
        return result.ToString();
    }

    private static IEnumerable<CodeTypeMembers> GetActionCodes(IHtmlElement element)
    {
        if (element is null)
        {
            yield break;
        }

        if (element is IHasSegregationAction segElement)
        {
            var actionCodes = segElement.GenerateCodeTypeMembers();
            if (actionCodes is not null)
            {
                foreach (var actionCode in actionCodes)
                {
                    yield return actionCode;
                }
            }
        }
        else if (element is IHasCustomAction cusElement)
        {
            var actionCodes = cusElement.GenerateCodeTypeMembers();
            if (actionCodes is not null)
            {
                foreach (var actionCode in actionCodes)
                {
                    yield return actionCode;
                }
            }
        }

        if (element.Children?.Any() == true)
        {
            foreach (var child in element.Children)
            {
                foreach (var code in BlazorComponentBase<TBlazorComponent>.GetActionCodes(child))
                {
                    yield return code;
                }
            }
        }
    }

    private static IEnumerable<CodeTypeMembers> GetCodeTypeMembers(IHtmlElement element, GenerateCodesParameters arguments)
    {
        if (element is ISupportsBehindCodeMember b)
        {
            var codes = b.GenerateTypeMembers(arguments);
            foreach (var code in codes)
            {
                yield return code;
            }
        }
        if (element.Children.Count != 0)
        {
            foreach (var child in element.Children)
            {
                foreach (var code in BlazorComponentBase<TBlazorComponent>.GetCodeTypeMembers(child, arguments))
                {
                    yield return code;
                }
            }
        }
    }

    private static string? returnType(string? type) => type?.EqualsTo("void") ?? false ? null : type;

    private GenerateCodeResult GenerateBehindCode(in GenerateCodesParameters? arguments)
    {
        var args = arguments ?? GenerateCodesParameters.FullCode();
        this.OnInitializingBehindCode(args);

        var mainUnit = new CodeCompileUnit();
        var mainClassType = createMainClassType(mainUnit);

        var partUnit = new CodeCompileUnit();
        var (partNameSpace, partClassType) = createPartClassType(partUnit);

        addFieldsToMainClass(mainClassType);
        addFieldsToPartClass(partClassType);

        var initializedAsyncMethodBody = new StringBuilder();
        if (initializedAsyncMethodBody.Length > 0)
        {
            addPageInitializedMethod(mainClassType, partClassType, initializedAsyncMethodBody);
        }
        addConstructor(partClassType, partNameSpace);

        addMethodsToMainClass(mainClassType);
        addMethodsToPartClass(partClassType);

        addPropertiesToPartClass(partClassType);
        addParametersToPartClass(partClassType);

        addChildren(args, mainClassType, partClassType);

        var mainCodeFileName = Path.ChangeExtension($"{this.Name}.tmp", this.MainCodeFileExtension);
        var mainClass = args.GenerateMainCode
            ? Code.New(this.Name, Languages.CSharp, CodeDomHelper.RemoveAutoGeneratedTag(mainUnit.GenerateCode()), false, mainCodeFileName)
            : null;

        var partCodeFileName = Path.ChangeExtension($"{this.Name}.tmp", this.PartialCodeFileExtension);
        var partClass = args.GeneratePartialCode
            ? Code.New(this.Name, Languages.CSharp, partUnit.GenerateCode(), true, partCodeFileName)
            : null;

        return new(mainClass, partClass);

        CodeTypeDeclaration createMainClassType(in CodeCompileUnit mainUnit)
        {
            var mainNameSpace = mainUnit.AddNewNameSpace(this.NameSpace);
            _ = mainNameSpace.UseNameSpace(this.AdditionalUsings);
            var mainClassType = createMainClass(mainNameSpace);
            foreach (var ns in this.MainCodeUsingNameSpaces)
            {
                _ = mainNameSpace.UseNameSpace(ns);
            }

            return mainClassType;
        }

        (CodeNamespace, CodeTypeDeclaration) createPartClassType(in CodeCompileUnit partUnit)
        {
            var partNameSpace = partUnit.AddNewNameSpace(this.NameSpace);
            _ = partNameSpace.UseNameSpace(this.AdditionalUsings);

            var partClassType = createPartialClass(partNameSpace);
            foreach (var ns in this.PartialCodeUsingNameSpaces)
            {
                _ = partNameSpace.UseNameSpace(ns);
            }

            return (partNameSpace, partClassType);
        }

        void addFieldsToPartClass(in CodeTypeDeclaration partClassType)
        {
            foreach (var field in this.Fields.Where(m => m.IsPartial))
            {
                _ = partClassType.AddField(field.Type, field.Name, field.Comment, field.AccessModifier, field.IsReadOnly, field.IsPartial);
            }
        }

        void addPropertiesToPartClass(in CodeTypeDeclaration partClassType)
        {
            foreach (var property in this.Properties.Where(x => x.Caption.IsNullOrEmpty()))
            {
                _ = partClassType.AddProperty(property.ToPropertyInfo());
            }
        }

        void addFieldsToMainClass(in CodeTypeDeclaration mainClassType)
        {
            foreach (var field in this.Fields.Where(m => !m.IsPartial))
            {
                _ = mainClassType.AddField(field.Type, field.Name, field.Comment, field.AccessModifier, field.IsReadOnly, field.IsPartial);
            }
        }

        void addConstructor(in CodeTypeDeclaration codeType, in CodeNamespace codeNs)
            => codeType.AddConstructor(Enumerable.Empty<(string, string, string)>());

        void addMethodsToPartClass(in CodeTypeDeclaration partClassType)
        {
            foreach (var method in this.Actions.OfType<ButtonActor>().Where(m => m.IsPartial == true || !m.Body.IsNullOrEmpty()))
            {
                _ = partClassType.AddMethod(method.EventHandlerName ?? method.Name.NotNull(), method.Body, returnType(method.ReturnType), method.AccessModifier, method.IsPartial == true, (method.Arguments ?? []).Select(x => (x.Type.FullPath, x.Name)).ToArray());
            }
            var onInitializedAsyncBody = Component_OnInitializedAsync_MethodBody(this.Actions.FirstOrDefault(m => m.Name == Keyword_AddToOnInitializedAsync && (m.IsPartial == true))?.Body);
            _ = partClassType.AddMethod("OnInitializedAsync", body: onInitializedAsyncBody, accessModifiers: MemberAttributes.Family | MemberAttributes.Override, returnType: "async Task");
            foreach (var method in this.Actions.OfType<FormActor>().Where(x => x.IsPartial is null or true))
            {
                _ = partClassType.AddMethod(method.EventHandlerName ?? method.Name.NotNull(), method.Body, returnType(method.ReturnType), method.AccessModifier, method.IsPartial == true, (method.Arguments ?? []).Select(x => (x.Type.FullPath, x.Name)).ToArray());
            }
        }
        void addMethodsToMainClass(in CodeTypeDeclaration mainClassType)
        {
            foreach (var method in this.Actions.OfType<ButtonActor>().Where(m => m.IsPartial == false && m.Body.IsNullOrEmpty()))
            {
                _ = mainClassType.AddMethod(method.EventHandlerName ?? method.Name.NotNull(), method.Body, returnType(method.ReturnType), method.AccessModifier, method.IsPartial == true, (method.Arguments ?? []).Select(x => (x.Type.FullPath, x.Name)).ToArray());
            }
            var onInitializedAsyncBody = this.Actions.FirstOrDefault(m => m.Name == Keyword_AddToOnInitializedAsync && (m.IsPartial == false))?.Body;
            _ = mainClassType.AddMethod("OnLoadAsync", body: onInitializedAsyncBody ?? DefaultTaskMethodBody, accessModifiers: MemberAttributes.Family | MemberAttributes.Override, returnType: "async Task");
            foreach (var method in this.Actions.OfType<FormActor>().Where(x => x.IsPartial is not true))
            {
            }
        }

        void addChildren(in GenerateCodesParameters arguments, in CodeTypeDeclaration mainClassType, in CodeTypeDeclaration partClassType)
        {
            if (this is BlazorPage)
            {
                return;
            }

            foreach (var child in this.Children)
            {
                //Note  (✔Fixed) 𝒯𝒽𝒾𝓈 𝓂𝓊𝓈𝓉 𝒷𝑒 𝒸𝒽𝒶𝓃𝑔𝑒𝒹 𝓁𝒶𝓉𝑒𝓇
                if (child is BlazorComponent)
                {
                    continue;
                }

                var actionCodes = BlazorComponentBase<TBlazorComponent>.GetActionCodes(child);
                foreach (var (mainMember, partMember) in actionCodes)
                {
                    if (mainMember is not null && !mainClassType.Members.Cast<CodeTypeMember>().Any(x => x.Name == mainMember.Name))
                    {
                        _ = mainClassType.Members.Add(mainMember);
                    }

                    if (partMember is not null && !partClassType.Members.Cast<CodeTypeMember>().Any(x => x.Name == partMember.Name))
                    {
                        _ = partClassType.Members.Add(partMember);
                    }
                }

                var members = BlazorComponentBase<TBlazorComponent>.GetCodeTypeMembers(child, arguments);
                foreach (var (mainMember, partMember) in members)
                {
                    if (mainMember is not null)
                    {
                        _ = mainClassType.Members.Add(mainMember);
                    }

                    if (partMember is not null)
                    {
                        _ = partClassType.Members.Add(partMember);
                    }
                }
            }
        }

        void addParametersToPartClass(in CodeTypeDeclaration partClassType)
        {
            _ = partClassType.AddProperty("BlazorApp.Shared.MessageComponent",
                                          "MessageComponent",
                                          getter: new(true, false),
                                          setter: new(true, false));
            foreach (var parameter in this.Parameters)
            {
                _ = partClassType.AddProperty(parameter.Type.FullPath,
                                              parameter.Name,
                                              getter: new(true, false),
                                              setter: new(true, false),
                                              isNullable: true,
                                              attributes: _parameterAttributes);
            }
        }

        void addPageInitializedMethod(in CodeTypeDeclaration mainClassType, in CodeTypeDeclaration partClassType, in StringBuilder initializedAsyncMethodBody)
        {
            _ = initializedAsyncMethodBody.AppendLine(InitializedAsyncMethodBody);
            _ = mainClassType.AddMethod("OnPageInitializedAsync", returnType: "async Task", accessModifiers: MemberAttributes.Private);
            var initializedAsyncBodyLines = initializedAsyncMethodBody.ToString().Split(Environment.NewLine);
            _ = initializedAsyncMethodBody.Clear();
            foreach (var line in initializedAsyncBodyLines)
            {
                _ = initializedAsyncMethodBody.AppendLine(line.TrimEnd().Add(8, before: true));
            }
            var initializedAsyncBodyCode = initializedAsyncMethodBody.ToString();
            _ = partClassType.AddMethod("OnInitializedAsync", body: initializedAsyncBodyCode.TrimEnd(), returnType: "async Task", accessModifiers: MemberAttributes.Family | MemberAttributes.Override);
        }

        CodeTypeDeclaration createPartialClass(in CodeNamespace partNameSpace)
            => partNameSpace.UseNameSpace(typeof(string).Namespace!)
                        .UseNameSpace(typeof(Enumerable).Namespace!)
                        .UseNameSpace(typeof(Task).Namespace!)
                        .UseNameSpace(typeof(ObservationRepository).Namespace!)
                        .AddNewType(this.Name, isPartial: true, baseTypes: this.GetBaseTypes().IsNullOrEmpty() ? null : new[] { this.GetBaseTypes()! });

        CodeTypeDeclaration createMainClass(in CodeNamespace mainNameSpace)
            => mainNameSpace.UseNameSpace(typeof(string).Namespace!)
                        .UseNameSpace(typeof(Enumerable).Namespace!)
                        .UseNameSpace(typeof(Task).Namespace!)
                        .AddNewType(this.Name, isPartial: true);
    }

    private Code GenerateUiCode(CodeCategory category, in GenerateCodesParameters? arguments = null)
    {
        this.OnInitializingUiCode(arguments);
        var result = this.OnGeneratingUiCode(arguments);
        result.props().Category = category;
        return result;
    }
}