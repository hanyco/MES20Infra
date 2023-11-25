using System.Diagnostics.CodeAnalysis;
using System.Text;

using Contracts.Services;

using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Helpers.CodeGen;
using Library.Results;

namespace Services;

internal sealed class MapperSourceGenerator(ILogger logger, ICodeGeneratorEngine codeGeneratorEngine) : IMapperSourceGenerator
{
    public Result<Codes> GenerateCodes([DisallowNull] MapperSourceGeneratorArguments args)
    {
        var srcType = TypePath.New(args.Source.Name, args.Source.NameSpace); // CQRS Output
        var dstType = TypePath.New($"{args.Destination.DbObject.Name}Dto", args.Destination.NameSpace); // Page ViewModel
        var dtoNameSpace = args.DtoNameSpace;

        var singleConverterMethod = new Method(args.MethodName)
        {
            IsExtension = args.IsExtension,
            Body = convertSingle_MethodBody(dstType.Name, args.InputArgumentName, args.Destination.Properties.Select(x => x.Name)),
            Parameters =
            {
                (srcType, args.InputArgumentName)
            },
            ReturnType = dstType
        };
        var listConverterMethod = new Method(singleConverterMethod.Name)
        {
            IsExtension = args.IsExtension,
            Body = convertEnumerable_MethodBody(singleConverterMethod.Name, StringHelper.Pluralize(args.InputArgumentName)),
            Parameters =
            {
                (TypePath.New(typeof(IEnumerable<>), [srcType]), StringHelper.Pluralize(args.InputArgumentName))
            },
            ReturnType = TypePath.New(typeof(List<>), [dstType])
        };

        var converterClass = new Class(args.ClassName)
        {
            AccessModifier = AccessModifier.Public,
            InheritanceModifier = InheritanceModifier.Static | InheritanceModifier.Partial
        }.AddMember(singleConverterMethod, listConverterMethod);

        var nameSpace = INamespace.New(dtoNameSpace);
        _ = nameSpace.AddType(converterClass)
            .UsingNamespaces.AddRange(srcType.GetNameSpaces()).AddRange(dstType.GetNameSpaces());

        var statement = codeGeneratorEngine.Generate(nameSpace);
        var formatted = RoslynHelper.ReformatCode(statement);
        var fileName = args.FileName ?? $"{converterClass.Name}.{srcType.Name}.{dstType.Name}{(args.IsPartial ? ".partial" : "")}.cs";
        var code = Code.New(converterClass.Name, Languages.CSharp, formatted, args.IsPartial, fileName)
            .With(x => x.props().Category = CodeCategory.Converter);

        return Result<Codes>.CreateSuccess(code.ToCodes());

        static string convertEnumerable_MethodBody(string singleConverterMethodName, string argName) =>
            $"return {argName}.Select({singleConverterMethodName}).ToList();";
        static string convertSingle_MethodBody(string dstClassName, string argName, IEnumerable<string?> propNames) =>
            new StringBuilder()
                .AppendLine($"var result = new {dstClassName}")
                .AppendLine($"{{")
                .AppendAllLines(propNames, propName => $"{propName} = {argName}.{propName},")
                .AppendLine($"}};")
                .AppendLine($"return result;")
                .Build();
    }
}