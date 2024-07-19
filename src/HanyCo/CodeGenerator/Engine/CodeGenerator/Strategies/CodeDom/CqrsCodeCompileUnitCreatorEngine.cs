using System.CodeDom;
using System.Globalization;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Bases;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Interfaces;
using HanyCo.Infra.CodeGeneration.CodeGenerator.Models.Components;
using HanyCo.Infra.CodeGeneration.Definitions;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.Helpers;
using HanyCo.Infra.Markers;
using HanyCo.Infra.Security.Markers;

using Library.CodeGeneration.Models;
using Library.Data.SqlServer.Builders.Bases;
using Library.Helpers.CodeGen;

namespace HanyCo.Infra.CodeGeneration.CodeGenerator.Strategies.CodeDom;

[Worker]
internal static class CqrsCodeCompileUnitCreatorEngine
{
    private const string AUTO_GENERATED_FILE_HEADER = "Created by HanyCo Infrastructure Code Generator with ♥";

    /// <summary>
    /// Generators the code.
    /// </summary>
    /// <param name="model">The CQRS scaffolding model.</param>
    /// <param name="cqrsType">Type of the CQRS.</param>
    /// <returns></returns>
    internal static IEnumerable<ICodeGeneratorUnit> Create(ICodeGenCqrsModel model)
    {
        var cqrsUnits = GenerateCqrsSegs(model);
        var dtoUnits = GenerateDtos(model);
        foreach (var cqrs in cqrsUnits)
        {
            yield return new CodeCompileUnitWrapper(cqrs.Unit, $"{cqrs.TypeName}", true).With(x => x.props().Category = cqrs.props().Category);
        }

        if (model.Segregates.Any(x => x.HasPartialClass))
        {
            var partialUnits = GeneratePartialCqrsSegs(model);
            foreach (var par in partialUnits)
            {
                yield return new CodeCompileUnitWrapper(par.Unit, $"{par.TypeName}", false).With(x => x.props().Category = par.props().Category);
            }
        }
        foreach (var dto in dtoUnits)
        {
            yield return new CodeCompileUnitWrapper(dto.Unit, $"{dto.TypeName}", true).With(x => x.props().Category = dto.props().Category);
        }
    }

    internal static ICodeGeneratorUnit Create(CodeGenDto dto, string? nameSpace)
    {
        var (unit, typeName) = GenerateDto(dto, nameSpace);
        return new CodeCompileUnitWrapper(unit, typeName.Name);
    }

    private static IEnumerable<(string PropTypeName, string PropName, string BackingFieldName)> AddProps<TContainer>(in TContainer container,
        in CodeTypeDeclaration @class,
        in CodeNamespace classNameSpace,
        string? dtoNameSpace)
        where TContainer : IPropertyContainer
    {
        var result = new List<(string PropTypeName, string PropName, string BackingFieldName)>();
        foreach (var prop in container.Properties)
        {
            var nameSpaces = prop.Type.Namespaces;
            if (prop.Type is CodeGenDto && !dtoNameSpace.IsNullOrEmpty())
            {
                nameSpaces = nameSpaces.AddImmuted(dtoNameSpace);
            }

            _ = classNameSpace.UseNameSpace(nameSpaces);

            var propType = prop.Type.Name;
            if (prop.IsList)
            {
                propType = $"List<{propType}>";
                _ = classNameSpace.UseNameSpace("System.Collections.Generic");
            }
            if (prop.IsNullable)
            {
                propType = $"{propType}?";
            }
            var propName = TypeMemberNameHelper.ToPropName(prop.Name);
            _ = @class.AddProperty(propType, propName, prop.Comment, setter: new(container is CodeGenDto, false));

            result.Add((propType, propName, TypeMemberNameHelper.ToFieldName(propName)));
        }

        return result.AsEnumerable();
    }

    private static CodeNamespace AddSegregateUsingNameSpaces(in ICodeGenCqrsModel model, in CodeCompileUnit unit)
        => unit.AddNewNameSpace(model.CqrsNameSpace);

    private static IEnumerable<GenerateCqrsSegsResult> GenerateCqrsSegs(ICodeGenCqrsModel model)
    {
        foreach (var seg in model.Segregates)
        {
            var unit = new CodeCompileUnit();//.AddRegion(AUTO_GENERATED_FILE_HEADER);
            var mainNameSpace = AddSegregateUsingNameSpaces(model, unit);
            var type = GenerateSegregate(seg, model.Name, mainNameSpace, model.DtoNameSpace);
            var result = new GenerateCqrsSegsResult(type.Name, unit);
            result.props().Category = seg.props().Category;
            yield return result;
        }
    }

    private static (CodeCompileUnit Unit, CodeTypeDeclaration Type) GenerateDto(CodeGenDto dto, string? modelDtoNameSpace = null)
    {
        var dtoNs = dto.Namespaces.Any()
                            ? dto.Namespaces.Count() == 1
                                ? dto.Namespaces.First()
                                : dto.Namespaces.ElementAt(1)
                            : modelDtoNameSpace is not null
                                ? modelDtoNameSpace
                                : string.Empty;
        var dtoUnit = new CodeCompileUnit().AddRegion(AUTO_GENERATED_FILE_HEADER);
        var dtoNameSpace = dtoUnit.AddNewNameSpace(dtoNs).UseNameSpace("System");
        var interfaceNameSpaces = dto.GetBaseTypes().Select(ns => ns.Namespaces).SelectAll();
        if (interfaceNameSpaces.Any())
        {
            foreach (var ns in interfaceNameSpaces)
            {
                if (ns is not null)
                {
                    _ = dtoNameSpace.UseNameSpace(ns);
                }
            }
        }

        var interfaceNames = dto.GetBaseTypes().Select(ns => ns.Name);
        var dtoClass = dtoNameSpace
            .AddNewType(dto.Name, interfaceNames, isPartial: true)
            .AddSummary(dto.Comment ?? $"The data transfer object of {dto.Name.Replace("DTO", "").Replace("Dto", "")}");
        _ = AddProps(dto, dtoClass, dtoNameSpace, dtoNs);
        return (dtoUnit, dtoClass);
    }

    private static IEnumerable<GenerateCqrsSegsResult> GenerateDtos(ICodeGenCqrsModel model)
    {
        var modelDtoNameSpace = model.DtoNameSpace;
        foreach (var dto in model.Dtos)
        {
            (var dtoUnit, var dtoClass) = GenerateDto(dto, modelDtoNameSpace);
            var result = new GenerateCqrsSegsResult(dtoClass.Name, dtoUnit);
            yield return result.With(x => x.props().Category = dto.props().Category);
        }
    }

    private static IEnumerable<GenerateCqrsSegsResult> GeneratePartialCqrsSegs(ICodeGenCqrsModel model)
    {
        foreach (var segregate in model.Segregates.Where(seg => seg.HasPartialClass))
        {
            var unit = new CodeCompileUnit().AddRegion(AUTO_GENERATED_FILE_HEADER);
            var parNameSpace = AddSegregateUsingNameSpaces(model, unit);
            var type = GeneratePartialSegregate(segregate, model.Name, parNameSpace);
            var result = new GenerateCqrsSegsResult(type.Name, unit);
            yield return result.With(x => x.props().Category = segregate.props().Category);
        }
    }

    private static CodeTypeDeclaration GeneratePartialSegregate<TSegregate>(in TSegregate segregate, string cqrsName, in CodeNamespace nameSpace)
            where TSegregate : ICodeGenCqrsSegregate
    {
        var segInterfaces = segregate.Interfaces;
        var interfaceNameSpaces = segInterfaces.Select(ns => ns.Namespaces).SelectAll();
        if (interfaceNameSpaces.Any())
        {
            foreach (var ns in interfaceNameSpaces)
            {
                if (ns is not null)
                {
                    _ = nameSpace.UseNameSpace(ns);
                }
            }
        }

        var className = $"{cqrsName}{segregate.Suffix}";
        var codeTypeDeclaration = nameSpace.AddNewType(className, null, isPartial: true);

        if (IsValidSegregatePartial(segregate, Partials.OnInitialize))
        {
            _ = codeTypeDeclaration.AddMethod(accessModifiers: MemberAttributes.ScopeMask, returnType: "partial void", name: "OnInitialize");
        }

        if (IsValidSegregatePartial(segregate, Partials.Handller))
        {
            _ = nameSpace.UseNameSpace("System.Threading.Tasks");
            _ = codeTypeDeclaration.AddMethod(
                    accessModifiers: MemberAttributes.Final | MemberAttributes.Public,
                    returnType: $"async Task<{cqrsName}Result>", name: "HandleAsync",
                    body: segregate.ExecuteBody?? $"            throw new System.NotImplementedException();",
                    arguments: ($"{cqrsName}Parameter", "parameter"));
        }

        return codeTypeDeclaration;
    }

    private static CodeTypeDeclaration GenerateSegregate(
        in ICodeGenCqrsSegregate segregate,
        string cqrsName,
        in CodeNamespace nameSpace,
        string? dtoNameSpace)
    {
        var segInterfaces = segregate.GetInterfaces(cqrsName).Select(i => CodeGenType.New(i.TrimEnd(".")));
        var interfaceNameSpaces = segInterfaces.Select(ns => ns.Namespaces).SelectAll();
        foreach (var ns in interfaceNameSpaces)
        {
            if (!ns.IsNullOrEmpty())
            {
                _ = nameSpace.UseNameSpace(ns.TrimEnd("."));
            }
        }

        var interfaceNames = segInterfaces.Select(ns => ns.FullName);
        var className = $"{cqrsName}{segregate.Suffix}";
        var segClass = nameSpace.AddNewType(className, interfaceNames, isPartial: true);
        _ = segClass.AddSummary(segregate.Comment ?? $"The {segregate.Suffix.ToLower(CultureInfo.InvariantCulture)} of {cqrsName}");

        var ctorParams = new List<(string Type, string Name, string DataMemberName)>();
        ctorParams.AddRange(AddProps(segregate, segClass, nameSpace, dtoNameSpace));

        if (IsValidSegregatePartial(segregate, Partials.OnInitialize))
        {
            _ = segClass.AddConstructor(ctorParams, "            this.OnInitialize();", comment: $@"Initializes a new instance of the <see cref=""{className}""/> class.");
            _ = segClass.AddPartialMethod("OnInitialize");
        }
        else
        {
            _ = segClass.AddConstructor(ctorParams, comment: $@"Initializes a new instance of the <see cref=""{className}""/> class.");
        }
        if (segClass.IsPartial && (segregate.SecurityKeys?.Any() ?? false))
        {
            var securityAttribute = typeof(SecurityDescriptorAttribute);
            nameSpace.UseNameSpace(securityAttribute.Namespace!);
            segClass.AddAttribute(securityAttribute.Name, (nameof(SecurityDescriptorAttribute.Key), segregate.SecurityKeys.First()));
        }

        return segClass;
    }

    private static bool IsValidSegregatePartial<TSegregate>(in TSegregate segregate, in Partials partials)
        where TSegregate : ICodeGenCqrsSegregate
        => segregate.GetValidPartials().Contains(partials) && segregate.GetPartials().Contains(partials);
}

public record GenerateCqrsSegsResult(string TypeName, CodeCompileUnit Unit)
{
    public static implicit operator (string TypeName, CodeCompileUnit Unit)(GenerateCqrsSegsResult value) =>
        (value.TypeName, value.Unit);
    public static implicit operator GenerateCqrsSegsResult((string TypeName, CodeCompileUnit Unit) value) =>
        new(value.TypeName, value.Unit);
}