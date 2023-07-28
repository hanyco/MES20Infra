﻿using System.CodeDom;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration.Models;
using Library.Data.Models;
using Library.DesignPatterns.Behavioral.Observation;
using Library.Exceptions.Validations;
using Library.Helpers.CodeGen;
using Library.Interfaces;
using Library.Validations;

namespace HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;

[Immutable]
[Fluent]
public abstract class BlazorComponentBase<TBlazorComponent> : IHtmlElement, ICodeGenerator, IComponentCodeUnit, IParent<IHtmlElement>
    where TBlazorComponent : BlazorComponentBase<TBlazorComponent>
{
    private static readonly string[] _parametersAttributes = new[] { "Microsoft.AspNetCore.Components.Parameter" };

    protected BlazorComponentBase(in string name) => this.Name = name;

    public IList<MethodActor> Actions { get; } = new List<MethodActor>();
    public Dictionary<string, string?> Attributes { get; } = new Dictionary<string, string?>();
    public IList<IHtmlElement> Children { get; } = new List<IHtmlElement>();
    public TypePath? DataContextType { get; private set; }
    public IList<FieldActor> Fields { get; } = new List<FieldActor>();
    public virtual string HtmlFileExtension { get; } = "razor";
    public string Indent { get; } = StringHelper.Space(4);
    public bool IsGrid { get; set; }
    public virtual string MainCodeFileExtension { get; } = "razor.cs";
    public IList<string> MainCodeUsingNameSpaces { get; } = new List<string>();
    public string Name { get; init; }

    public string? NameSpace { get; private set; }

    public IList<MethodArgument> Parameters { get; } = new List<MethodArgument>();
    public virtual string PartialCodeFileExtension { get; } = "partial.cs";

    public IList<string> PartialCodeUsingNameSpaces { get; } = new List<string>();

    public BootstrapPosition Position { get; private set; } = new();

    public IList<PropertyActor> Properties { get; } = new List<PropertyActor>();

    public GenerateCodeResult GenerateBehindCode(in GenerateCodesParameters? arguments)
    {
        var args = arguments ?? new();
        this.OnInitializingBehindCode(args);

        var mainUnit = new CodeCompileUnit();
        var mainClassType = createMainClassType(mainUnit);

        var partUnit = new CodeCompileUnit();
        var (partNameSpace, partClassType) = createPartClassType(partUnit);

        var initializedAsyncMethodBody = new StringBuilder();
        initializeDataContext(initializedAsyncMethodBody);

        if (initializedAsyncMethodBody.Length > 0)
        {
            addPageInitializedMethod(mainClassType, partClassType, initializedAsyncMethodBody);
        }

        addMethodsToMainClass(mainClassType);
        addMethodsToPartClass(partClassType);

        addFieldsToMainClass(mainClassType);
        addFieldsToPartClass(partClassType);

        addPropertiesToPartClass(partClassType);
        addParametersToPartClass(partClassType);

        addChildren(args, mainClassType, partClassType);

        addConstructor(partClassType, partNameSpace);

        var mainCodeFileName = Path.ChangeExtension($"{this.Name}.tmp", this.MainCodeFileExtension);
        var mainClass = args.GenerateMainCode
            ? Code.ToCode(this.Name, Languages.CSharp, CodeDomHelper.RemoveAutoGeneratedTag(mainUnit.GenerateCode()), false, mainCodeFileName)
            : null;

        var partCodeFileName = Path.ChangeExtension($"{this.Name}.tmp", this.PartialCodeFileExtension);
        var partClass = args.GeneratePartialCode
            ? Code.ToCode(this.Name, Languages.CSharp, partUnit.GenerateCode(), true, partCodeFileName)
            : null;

        return new(mainClass, partClass);

        CodeTypeDeclaration createMainClassType(in CodeCompileUnit mainUnit)
        {
            var mainNameSpace = mainUnit.AddNewNameSpace(this.NameSpace);
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
            var partClassType = createPartialClass(partNameSpace);
            foreach (var ns in this.PartialCodeUsingNameSpaces)
            {
                _ = partNameSpace.UseNameSpace(ns);
            }

            return (partNameSpace, partClassType);
        }

        void initializeDataContext(in StringBuilder onInitializedAsyncBody)
            => this.OnInitializeDataContext(onInitializedAsyncBody);

        void addPropertiesToPartClass(in CodeTypeDeclaration partClassType)
        {
            foreach (var property in this.Properties)
            {
                _ = partClassType.AddProperty(property.ToPropertyInfo());
            }
        }

        void addFieldsToPartClass(in CodeTypeDeclaration partClassType)
        {
            foreach (var field in this.Fields.Where(m => m.IsPartial))
            {
                _ = partClassType.AddField(field.ToFieldInfo());
            }
        }

        void addFieldsToMainClass(in CodeTypeDeclaration mainClassType)
        {
            foreach (var field in this.Fields.Where(m => !m.IsPartial))
            {
                _ = mainClassType.AddField(field.ToFieldInfo());
            }
        }

        void addConstructor(in CodeTypeDeclaration codeType, in CodeNamespace codeNs)
            => codeType.AddConstructor(Enumerable.Empty<(string, string, string)>());

        void addMethodsToPartClass(in CodeTypeDeclaration partClassType)
        {
            foreach (var method in this.Actions.Where(m => m.IsPartial))
            {
                _ = partClassType.AddMethod(method.EventHandlerName ?? method.Name, method.Body, method.ReturnType, method.AccessModifier, method.IsPartial, method.Arguments?.ToArray() ?? Array.Empty<MethodArgument>());
            }
        }

        void addMethodsToMainClass(in CodeTypeDeclaration mainClassType)
        {
            foreach (var method in this.Actions.Where(m => !m.IsPartial))
            {
                _ = mainClassType.AddMethod(method.EventHandlerName ?? method.Name, method.Body, method.ReturnType, method.AccessModifier, method.IsPartial, method.Arguments?.ToArray() ?? Array.Empty<MethodArgument>());
            }
            _ = mainClassType.AddMethod("OnInitializedAsync", accessModifiers: MemberAttributes.Family | MemberAttributes.Override, returnType: "async Task");
        }

        void addChildren(in GenerateCodesParameters arguments, in CodeTypeDeclaration mainClassType, in CodeTypeDeclaration partClassType)
        {
            if (this is BlazorPage)
            {
                return;
            }

            foreach (var child in this.Children)
            {
                //! 𝒯𝒽𝒾𝓈 𝓂𝓊𝓈𝓉 𝒷𝑒 𝒸𝒽𝒶𝓃𝑔𝑒𝒹 𝓁𝒶𝓉𝑒𝓇
                if (child is BlazorComponent)
                {
                    continue;
                }

                var actionCodes = this.GetActionCodes(child);
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

                var members = this.GetCodeTypeMembers(child, arguments);
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
            foreach (var parameter in this.Parameters)
            {
                _ = partClassType.AddProperty(parameter.Type,
                                              parameter.Name,
                                              getter: new(true, false),
                                              setter: new(true, false),
                                              isNullable: true,
                                              attributes: _parametersAttributes);
            }
        }

        void addPageInitializedMethod(in CodeTypeDeclaration mainClassType, in CodeTypeDeclaration partClassType, in StringBuilder initializedAsyncMethodBody)
        {
            _ = initializedAsyncMethodBody.AppendLine($"{this.Indent}await this.OnPageInitializedAsync();");
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
            => partNameSpace.UseNameSpace("System")
                        .UseNameSpace("System.Linq")
                        .UseNameSpace(typeof(Task).Namespace!)
                        .UseNameSpace(typeof(ObservationRepository).Namespace!)
                        .AddNewClass(this.Name, isPartial: true, baseTypes: this.GetBaseTypes().IsNullOrEmpty() ? null : new[] { this.GetBaseTypes()! });

        CodeTypeDeclaration createMainClass(in CodeNamespace mainNameSpace)
            => mainNameSpace.UseNameSpace("System")
                        .UseNameSpace("System.Linq")
                        .UseNameSpace(typeof(Task).Namespace!)
                        .AddNewClass(this.Name, isPartial: true);
    }

    public Codes GenerateCodes(in GenerateCodesParameters? arguments = null)
    {
        var args = arguments ?? new GenerateCodesParameters();
        _ = validate(args);
        var codes = new List<Code>();
        if (args.GenerateUiCode)
        {
            var htmlCode = this.GenerateUiCode(args);
            if (htmlCode is { } c)
            {
                codes.Add(c);
            }
        }
        if (args.GenerateMainCode || args.GeneratePartialCode)
        {
            var (mainCs, partialCs) = this.GenerateBehindCode(args);

            if (mainCs is { } c2)
            {
                codes.Add(c2);
            }

            if (partialCs is { } c3)
            {
                codes.Add(c3);
            }
        }

        return new Codes(codes);

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

    public Code GenerateUiCode(in GenerateCodesParameters? arguments = null)
    {
        this.OnInitializingUiCode(arguments);
        return this.OnGeneratingUiCode(arguments);
    }

    public TBlazorComponent SetDataContext(TypePath value)
        => this.This(() => this.DataContextType = value);

    public TBlazorComponent SetIsGrid(bool isGrid)
        => this.This(() => this.IsGrid = isGrid);

    public TBlazorComponent SetNameSpace(string? value)
        => this.This(() => this.NameSpace = value);

    public TBlazorComponent SetPosition(int? order = null, int? row = null, int? col = null, int? colSpan = null)
        => this.This(() => this.Position = new(order, row, col, colSpan));

    protected virtual string? GetBaseTypes()
        => null;

    protected virtual StringBuilder OnGeneratingHtmlCode(StringBuilder codeStringBuilder)
        => codeStringBuilder;

    protected virtual Code OnGeneratingUiCode(in GenerateCodesParameters? arguments = null)
    {
        Func<StringBuilder, StringBuilder> generate = this.IsGrid ? generateGridCode : generateDetailsCode;
        return generateHtmlCode(generate);

        StringBuilder generateDetailsCode(StringBuilder sb) => this.Children.GenerateChildrenCode(sb);
        StringBuilder generateGridCode(StringBuilder sb)
        {
            var columns = this.Properties.Select(x => new DataColumnBindingInfo(x.Caption, x.Name))
                .AddImmuted(new("Actions", this.Actions.Select(x => new BlazorButton(x.Name, x.Name, body: x.Caption) { OnClick = $"() => this.{x.EventHandlerName}(item.Id)" })));
            var table = BlazorTable.New($"{this.DataContextType}Grid", $"{this.DataContextType}Grid")
                .SetDataContextName("this.DataContext")
                .SetDataColumns(columns.ToArray());
            return sb.Append(table.GenerateUiCode().Statement);
        }

        Code generateHtmlCode(Func<StringBuilder, StringBuilder> generate)
        {
            var statementBuilder = New<StringBuilder>();
            var statement = statementBuilder.Fluent(this.OnGeneratingHtmlCode)
                .With(generate)
                .ToString();

            if (statement.IsNullOrEmpty())
            {
                return Code.Empty;
            }

            var htmlFileName = Path.ChangeExtension($"{this.Name}.tmp", this.HtmlFileExtension);
            return Code.ToCode(this.Name, Languages.BlazorCodeBehind, statement, false, htmlFileName);
        }
    }

    protected virtual void OnInitializeDataContext(in StringBuilder onInitializedAsyncBody)
    {
    }

    protected virtual void OnInitializingBehindCode(GenerateCodesParameters? arguments)
    {
    }

    protected virtual void OnInitializingUiCode(GenerateCodesParameters? arguments)
    {
    }

    protected TBlazorComponent This()
        => (this as TBlazorComponent)!;

    protected TBlazorComponent This(Action action)
        => this.This().Fluent(action);

    private IEnumerable<GenerateCodeTypeMemberResult> GetActionCodes(IHtmlElement element)
    {
        if (element is IHasHtmlAction hha)
        {
            var actionCodes = hha.GenerateActionCodes();
            if (actionCodes is not null)
            {
                foreach (var actionCode in actionCodes)
                {
                    yield return actionCode;
                }
            }
        }
        if (element.Children.Count != 0)
        {
            foreach (var child in element.Children)
            {
                foreach (var code in this.GetActionCodes(child))
                {
                    yield return code;
                }
            }
        }
    }

    private IEnumerable<GenerateCodeTypeMemberResult> GetCodeTypeMembers(IHtmlElement element, GenerateCodesParameters arguments)
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
                foreach (var code in this.GetCodeTypeMembers(child, arguments))
                {
                    yield return code;
                }
            }
        }
    }
}