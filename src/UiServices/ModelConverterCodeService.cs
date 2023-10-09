using System.Diagnostics.CodeAnalysis;

using Contracts.Services;

using HanyCo.Infra.CodeGeneration.Definitions;

using Library.CodeGeneration;
using Library.CodeGeneration.Models;
using Library.CodeGeneration.v2;
using Library.CodeGeneration.v2.Back;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

namespace Services;

internal sealed class ModelConverterCodeService(ICodeGeneratorEngine codeGenerator) : IBusinessService, IModelConverterCodeService
{
    private readonly ICodeGeneratorEngine _codeGenerator = codeGenerator;

    public Result<Codes> GenerateCodes(ModelConverterCodeParameter args)
    {
        if (!validate(args).TryParse(out var vr))
        {
            return vr.WithValue(Codes.Empty);
        }
        (var dto, var srcClass, var dstClass, var methodName) = args;
        methodName ??= CodeConstants.Converter_Convert_MethodName(dstClass.Name!);

        var ma = createSingleDtoConverter(dto, srcClass, dstClass, methodName);
        var mb = createEnumerableConverter(srcClass, dstClass, methodName);
        var cl = createExtensionClassAndAddMembers(srcClass, ma, mb);
        var ns = createNameSpace(dto);
        AddClassToNameSpace(cl, ns);
        AddUsings(dto, ma, mb, ns);

        var codeGenRes = this._codeGenerator.Generate(ns);
        if (codeGenRes.IsFailure)
        {
            return codeGenRes.WithValue(Codes.Empty);
        }

        var result = Code.New(methodName, Languages.CSharp, codeGenRes, true, $"ModelConverter.{srcClass}.{methodName}.cs")
            .With(x => x.props().Category = CodeCategory.Converter)
            .ToCodes();
        return Result<Codes>.CreateSuccess(result);

        [return: NotNull]
        static Result validate([NotNull] ModelConverterCodeParameter? viewModel) => viewModel.ArgumentNotNull()
                .Check()
                .NotNull(x => x.SourceDto)
                .NotNull(x => x.SourceDto.Name)
                .Build();

        static Method createSingleDtoConverter(Contracts.ViewModels.DtoViewModel src, TypePath srcClass, TypePath dstClass, string methodName) =>
            new Method(methodName)
            {
                IsExtension = true,
                Body = CodeConstants.Converter_ConvertSingle_MethodBody(srcClass.Name!, dstClass.Name!, "o", src.Properties.Select(x => x.Name)),
                ReturnType = dstClass
            }.AddParameter(srcClass.Name!, "o");

        static Method createEnumerableConverter(TypePath srcClass, TypePath dstClass, string methodName) =>
            new Method(methodName)
            {
                IsExtension = true,
                Body = CodeConstants.Converter_ConvertEnumerable_MethodBody(srcClass.Name!, dstClass.Name!, "o"),
                ReturnType = $"System.IEnumerable<{dstClass.Name}>"
            }.AddParameter($"System.IEnumerable<{srcClass.Name}>", "o");

        static Class createExtensionClassAndAddMembers(TypePath srcClass, Method ma, Method mb) =>
            new Class(srcClass.Name!) { InheritanceModifier = InheritanceModifier.Partial | InheritanceModifier.Static }
                    .AddMember(ma)
                    .AddMember(mb);

        static INamespace createNameSpace(Contracts.ViewModels.DtoViewModel src) =>
            INamespace.New($"{src.NameSpace}.Converters");

        static void AddClassToNameSpace(Class cl, INamespace ns) =>
            ns.AddType(cl);

        static void AddUsings(Contracts.ViewModels.DtoViewModel src, Method ma, Method mb, INamespace ns)
        {
            if (src.NameSpace != null)
            {
                _ = ns.AddUsingNameSpace(src.NameSpace);
            }

            if (ma.ReturnType?.NameSpace != null)
            {
                _ = ns.AddUsingNameSpace(ma.ReturnType.NameSpace);
            }

            if (mb.ReturnType?.NameSpace != null)
            {
                _ = ns.AddUsingNameSpace(mb.ReturnType.NameSpace);
            }

            _ = ns.AddUsingNameSpace(ma.Parameters.Select(x => TypePath.GetNameSpace(x.Type)));
            _ = ns.AddUsingNameSpace(mb.Parameters.Select(x => TypePath.GetNameSpace(x.Type)));
        }
    }
}