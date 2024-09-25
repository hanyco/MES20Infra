using System.CodeDom;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Components;
using HanyCo.Infra.CodeGeneration.FormGenerator.Html.Elements;
using HanyCo.Infra.CodeGeneration.Helpers;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
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

    protected BlazorComponentBase(string name, ICodeGeneratorEngine codeGenerator) 
        => (this.Name, this.CodeGenerator) = (name, codeGenerator);

    public IList<MethodActor> Actions { get; } = [];
    public HashSet<string> AdditionalUsings { get; } = [];
    public IDictionary<string, string?> Attributes { get; } = new Dictionary<string, string?>();
    Dictionary<string, string?> IHtmlElement.Attributes { get; }
    public IList<IHtmlElement> Children { get; } = [];
    public TypePath? DataContextType { get; set; }
    public IList<FieldInfo> Fields { get; } = [];
    public virtual string HtmlFileExtension { get; } = "razor";
    public bool IsGrid { get; set; }
    public virtual string MainCodeFileExtension { get; } = "razor.cs";
    public IList<string> MainCodeUsingNameSpaces { get; } = [];
    public string Name { get; init; }
    public string NameSpace { get; set; }
    public IList<MethodArgument> Parameters { get; } = [];
    public virtual string PartialCodeFileExtension { get; } = "razor.partial.cs";
    public IList<string> PartialCodeUsingNameSpaces { get; } = [];
    public BootstrapPosition Position { get => this._position ??= new(); set => this._position = value; }
    public IList<PropertyActor> Properties { get; } = [];
    public ICodeGeneratorEngine CodeGenerator { get; }

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
                        ? new BlazorCqrsButton(name: method.Name, body: method.Caption, onClick: method.EventHandlerName, onClickReturnType: ReturnType(method.ReturnType))
                                .With(x => x.Position.SetCol(1))
                        : new BlazorCustomButton(name: method.Name, body: method.Caption, onClick: method.EventHandlerName) { OnClickReturnType = ReturnType(method.ReturnType) }
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

    private static IEnumerable<ClassMembers> GetActionCodes(IHtmlElement element)
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

    private static IEnumerable<ClassMembers> GetCodeTypeMembers(IHtmlElement element, GenerateCodesParameters arguments)
    {
        if (element is ISupportsBehindCodeMember b)
        {
            var members = b.GenerateTypeMembers(arguments);
            foreach (var code in members)
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

    private static string? ReturnType(string? type) => type?.EqualsTo("void") ?? false ? null : type;

    private GenerateCodeResult GenerateBehindCode(in GenerateCodesParameters? arguments)
    {
        var args = arguments ?? GenerateCodesParameters.FullCode();
        this.OnInitializingBehindCode(args);

        var mainNameSpace = INamespace.New(this.NameSpace);
        var mainClassType = createMainClassType(mainNameSpace);
        
        var partNameSpace = INamespace.New(this.NameSpace);
        var partClassType = createPartClassType(partNameSpace);

        addFieldsToMainClass(mainClassType);
        addFieldsToPartClass(partClassType);
                
        addConstructor(partClassType);

        addMethodsToMainClass(mainClassType);
        addMethodsToPartClass(partClassType);

        addPropertiesToPartClass(partClassType);
        addParametersToPartClass(partClassType);

        addChildren(args, mainClassType, partClassType);

        var mainCodeFileName = Path.ChangeExtension($"{this.Name}.tmp", this.MainCodeFileExtension);
        var partCodeFileName = Path.ChangeExtension($"{this.Name}.tmp", this.PartialCodeFileExtension);
        
        var mainClassCode = args.GenerateMainCode
            ? this.CodeGenerator.Generate(mainNameSpace, this.Name, Languages.CSharp, true, mainCodeFileName)
            : null;
        var partClassCode = args.GeneratePartialCode
            ? this.CodeGenerator.Generate(partNameSpace, this.Name, Languages.CSharp, true, partCodeFileName)
            : null;

        return new(mainClassCode!.Value, partClassCode!.Value);

        Class createMainClassType(in INamespace mainNameSpace)
        {
            _ = mainNameSpace.AddUsingNameSpace(this.AdditionalUsings)
                .AddUsingNameSpace(this.MainCodeUsingNameSpaces)
                .AddUsingNameSpace(typeof(string).Namespace!)
                .AddUsingNameSpace(typeof(Enumerable).Namespace!)
                .AddUsingNameSpace(typeof(Task).Namespace!);
            var mainClass =  new Class(this.Name) { InheritanceModifier = InheritanceModifier.Partial };
            mainNameSpace.AddType(mainClass);

            return mainClass;

        }        

        Class createPartClassType(in INamespace partNamespace)
        {
            partNameSpace.AddUsingNameSpace(this.NameSpace)
                .AddUsingNameSpace(this.AdditionalUsings);

            var partClassType = createPartialClass(partNameSpace);
            foreach (var ns in this.PartialCodeUsingNameSpaces)
            {
                _ = partNameSpace.AddUsingNameSpace(ns);
            }

            return partClassType;
        }
        
        void addFieldsToPartClass(in Class partClass)
        {
            foreach (var field in this.Fields.Where(m => m.IsPartial))
            {
                _ = partClass.AddField(field.Name, field.Type);
            }
        }

        void addPropertiesToPartClass(in Class partClass)
        {
            foreach (var property in this.Properties.Where(x => x.Caption.IsNullOrEmpty()))
            {
                var prop = new CodeGenProperty(property.Name, property.Type,
                    setter: property.Setter is { } and { Has: true } ? new() : null,
                    getter: property.Getter is { } and { Has: true } ? new() : null);
                _ = partClass.AddProperty(prop);
            }
        }

        void addFieldsToMainClass(in Class mainClass)
        {
            foreach (var field in this.Fields.Where(m => !m.IsPartial))
            {
                _ = mainClass.AddField(field.Name, field.Type);
            }
            
        }

        void addConstructor(in Class codeType)
        {
            var ctor = new Method(codeType.Name)
            {
                IsConstructor = true,
            };
        }

        void addMethodsToPartClass(in Class partClass)
        {
            foreach (var method in this.Actions.OfType<ButtonActor>().Where(m => m.IsPartial == true || !m.Body.IsNullOrEmpty()))
            {
                var m = new Method(method.EventHandlerName ?? method.Name.NotNull())
                {
                    Body = method.Body,
                    InheritanceModifier = InheritanceModifier.Partial,
                    ReturnType = method.ReturnType is not null ? TypePath.New(method.ReturnType) : null
                }.AddArgument((method.Arguments ?? []).Select(x => (x.Type, x.Name)));
                partClass.AddMethod(m);
            }
            var onInitializedAsyncBody = Component_OnInitializedAsync_MethodBody(this.Actions.FirstOrDefault(m => m.Name == Keyword_AddToOnInitializedAsync && (m.IsPartial == true))?.Body);
            var onInitializedAsyncMethod = new Method("OnInitializedAsync")
            {
                Body = onInitializedAsyncBody,
                AccessModifier = AccessModifier.Protected,
                InheritanceModifier = InheritanceModifier.Override,
                ReturnType = "async Task"
            };
            partClass.AddMethod(onInitializedAsyncMethod);

            foreach (var method in this.Actions.OfType<FormActor>().Where(x => x.IsPartial is null or true))
            {
                var m = new Method(method.EventHandlerName ?? method.Name.NotNull())
                {
                    Body = method.Body,
                    ReturnType = method.ReturnType is null ? null : TypePath.New(method.ReturnType),
                    InheritanceModifier = method.IsPartial is true ? InheritanceModifier.Partial : InheritanceModifier.None
                }.AddArgument((method.Arguments ?? []).Select(x => (x.Type, x.Name)));
                _ = partClass.AddMethod(m);
            }
        }

        void addMethodsToMainClass(in Class mainClass)
        {
            foreach (var method in this.Actions.OfType<ButtonActor>().Where(m => m.IsPartial == false && m.Body.IsNullOrEmpty()))
            {
                var m = new Method(method.EventHandlerName ?? method.Name.NotNull())
                {
                    Body = method.Body,
                    InheritanceModifier = method.IsPartial == true ? InheritanceModifier.Partial : InheritanceModifier.None,
                    ReturnType = method.ReturnType is not null ? TypePath.New(method.ReturnType) : null
                }.AddArgument((method.Arguments ?? []).Select(x => (x.Type, x.Name)));
                mainClass.AddMethod(m);
            }
            var OnLoadAsyncBody = this.Actions.FirstOrDefault(m => m.Name == Keyword_AddToOnInitializedAsync && (m.IsPartial == false))?.Body;
            var OnLoadAsyncMethod = new Method("OnLoadAsync") 
            {
                AccessModifier = AccessModifier.Protected,
                InheritanceModifier = InheritanceModifier.Override,
                Body = OnLoadAsyncBody ?? DefaultTaskMethodBody,
                ReturnType = "async Task"
            };
            foreach (var method in this.Actions.OfType<FormActor>().Where(x => x.IsPartial is not true))
            {
            }
        }

        void addChildren(in GenerateCodesParameters arguments, in Class mainClass, in Class partClass)
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
                    if (mainMember is not null && !mainClass.Members.Any(x => x.Name == mainMember.Name))
                    {
                        _ = mainClass.Members.Add(mainMember);
                    }

                    if (partMember is not null && !partClass.Members.Any(x => x.Name == partMember.Name))
                    {
                        _ = partClass.Members.Add(partMember);
                    }
                }

                var members = BlazorComponentBase<TBlazorComponent>.GetCodeTypeMembers(child, arguments);
                foreach (var (mainMember, partMember) in members)
                {
                    if (mainMember is not null)
                    {
                        _ = mainClass.Members.Add(mainMember);
                    }

                    if (partMember is not null)
                    {
                        _ = partClass.Members.Add(partMember);
                    }
                }
            }
        }

        void addParametersToPartClass(in Class partClass)
        {
            var messageComponent = new CodeGenProperty("MessageComponent", "Web.UI.Components.Shared.MessageComponent", setter: new(), getter: new());
            _ = partClass.AddProperty(messageComponent);
            foreach (var parameter in this.Parameters)
            {
                var p = new CodeGenProperty(parameter.Name, new TypePath(parameter.Type.FullPath, null, true))
                    .AddAttribute("Microsoft.AspNetCore.Components.Parameter");
                _ = partClass.AddProperty(p);
            }
        }

        //void addPageInitializedMethod(in CodeTypeDeclaration mainClassType, in CodeTypeDeclaration partClassType, in StringBuilder initializedAsyncMethodBody)
        //{
        //    _ = initializedAsyncMethodBody.AppendLine(InitializedAsyncMethodBody);
        //    _ = mainClassType.AddMethod("OnPageInitializedAsync", returnType: "async Task", accessModifiers: MemberAttributes.Private);
        //    var initializedAsyncBodyLines = initializedAsyncMethodBody.ToString().Split(Environment.NewLine);
        //    _ = initializedAsyncMethodBody.Clear();
        //    foreach (var line in initializedAsyncBodyLines)
        //    {
        //        _ = initializedAsyncMethodBody.AppendLine(line.TrimEnd().Add(8, before: true));
        //    }
        //    var initializedAsyncBodyCode = initializedAsyncMethodBody.ToString();
        //    _ = partClassType.AddMethod("OnInitializedAsync", body: initializedAsyncBodyCode.TrimEnd(), returnType: "async Task", accessModifiers: MemberAttributes.Family | MemberAttributes.Override);
        //}

        Class createPartialClass(in INamespace partNameSpace)
        {
            partNameSpace.AddUsingNameSpace(typeof(string).Namespace!, typeof(Enumerable).Namespace!, typeof(Task).Namespace!, typeof(ObservationRepository).Namespace!);
            var partialClass = new Class(this.Name)
            {
                InheritanceModifier = InheritanceModifier.Partial,
            };
            var baseType = this.GetBaseTypes();
            if (!baseType.IsNullOrEmpty())
                partialClass.AddBaseType(TypePath.New(baseType));
            partNameSpace.AddType(partialClass);
            return partialClass;
        }
    }

    private Code GenerateUiCode(CodeCategory category, in GenerateCodesParameters? arguments = null)
    {
        this.OnInitializingUiCode(arguments);
        var result = this.OnGeneratingUiCode(arguments);
        result.props().Category = category;
        return result;
    }
}