using System.Diagnostics.CodeAnalysis;
using System.Text;

using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Helpers.CodeGen;
using Library.Results;
using Library.Validations;

namespace Services.CodeGen;

internal sealed class MapperSourceGenerator(ICodeGeneratorEngine codeGeneratorEngine) : IMapperSourceGenerator
{
    public Result<Codes> GenerateCodes([DisallowNull] MapperSourceGeneratorArguments args)
    {
        var validate = args.Check()
            .ArgumentNotNull()
            .NotNull(x => x.Source)
            .NotNull(x => x.Destination)
            .NotNull(x => x.Source.Model)
            .NotNull(x => x.Destination.Model)
            .NotNull(x => x.NameSpace);
        if (!validate.TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }

        var (srcModel, srcType) = args.Source; // CQRS Output
        var (dstModel, dstType) = args.Destination; // Page ViewModel
        srcType ??= TypePath.New(srcModel.Name, srcModel.NameSpace);
        dstType ??= TypePath.New($"{dstModel.DbObject.Name}Dto", dstModel.NameSpace);

        var converterClass = createClass(args.ClassName);
        var singleConverterMethod = createSingleConverterMethod(args, srcType, dstType);
        _ = converterClass.AddMember(singleConverterMethod);
        if (args.GenerateListConverter)
        {
            var listConverterMethod = createListConverterMethod(args, srcType, dstType);
            _ = converterClass.AddMember(listConverterMethod);
        }
        var nameSpace = INamespace.New(args.NameSpace)
            .AddUsingNameSpace(srcType.GetNameSpaces())
            .AddUsingNameSpace(dstType.GetNameSpaces())
            .AddUsingNameSpace(args.Source.Model.Properties.Select(x => TypePath.New(x.TypeFullName).GetNameSpaces()).SelectAll())
            .AddUsingNameSpace(args.Destination.Model.Properties.Select(x => TypePath.New(x.TypeFullName).GetNameSpaces()).SelectAll())
            .AddType(converterClass);

        var fileName = args.FileName ?? $"{args.ClassName}.{args.MethodName}.{srcType.Name}.{dstType.Name}{(args.IsPartial ? ".partial" : "")}.cs";

        var statement = codeGeneratorEngine.Generate(nameSpace);
        var code = new Code(args.ClassName, Languages.CSharp, RoslynHelper.ReformatCode(statement), args.IsPartial, fileName)
            .With(x =>
            {
                x.props().Category = CodeCategory.Converter;
                return x;
            });
        return Result.From(statement, code.ToCodes());

        static Class createClass(string name) =>
            new(name)
            {
                AccessModifier = AccessModifier.Public,
                InheritanceModifier = InheritanceModifier.Static | InheritanceModifier.Partial
            };
        static Method createSingleConverterMethod(MapperSourceGeneratorArguments args, TypePath srcType, TypePath dstType) =>
            new(args.MethodName)
            {
                IsExtension = args.IsExtension,
                Body = convertSingle_MethodBody(dstType.Name, args.InputArgumentName,
                    args.Destination.Model.Properties.Select(x => x.Name).Intersect(args.Source.Model.Properties.Select(x => x.Name))),
                Arguments =
                {
                    new(srcType, args.InputArgumentName)
                },
                ReturnType = dstType
            };
        static Method createListConverterMethod(MapperSourceGeneratorArguments args, TypePath srcType, TypePath dstType) =>
            new(args.MethodName)
            {
                IsExtension = args.IsExtension,
                Body = convertEnumerable_MethodBody(args.MethodName, StringHelper.Pluralize(args.InputArgumentName)),
                Arguments =
                {
                    new(TypePath.New(typeof(IEnumerable<>).FullName!, [srcType]), StringHelper.Pluralize(args.InputArgumentName))
                },
                ReturnType = TypePath.New(typeof(List<>).FullName!, [dstType])
            };
        static string convertEnumerable_MethodBody(string singleConverterMethodName, string argName) =>
            $"return {argName}.Select({singleConverterMethodName}).ToList();";
        static string convertSingle_MethodBody(string dstClassName, string argName, IEnumerable<string?> propNames) =>
            new StringBuilder()
                .AppendLine($"var result = new {dstClassName}")
                .AppendLine($"{{")
                .AppendAllLines(propNames, propName => $"{propName} = {argName}.{propName},")
                .AppendLine($"}};")
                .AppendLine($"return result;")
                .ToString();
    }
}